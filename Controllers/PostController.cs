using Microsoft.AspNetCore.Mvc;
using SoundSee.Database;
using SoundSee.Models;
using SoundSee.ViewModels;
using System.IO;
using System.IO.Compression;
using static System.Net.Mime.MediaTypeNames;

namespace SoundSee.Controllers
{
    public class PostController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SoundSeeDbContext _dbContext;

        public PostController(IWebHostEnvironment hostingEnvironment, SoundSeeDbContext dbContext) 
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("User/CreatePost")]
        public IActionResult SendToPost(PostViewModel model)
        {
            return View("~/Views/User/CreatePost.cshtml", model);
        }

        [HttpGet]
        public IActionResult StartPost(PostViewModel model)
        {
            model.StepCounter = 1;

            return View("~/Views/User/CreatePost.cshtml", model);
        }

        [HttpPost]
        public IActionResult ReviewPost(PostViewModel model)
        {
            Post post = new Post();
            int loopCount = 0;
            model.StepCounter = 2;

            post.Title = Request.Form["Title"];
            post.Description = Request.Form["Description"];
            post.Title = post.Title.Trim();
            post.Description = post.Description.Trim();

            // Collecting the 3 possible photos
            foreach (IFormFile files in Request.Form.Files)
            {
                using (var binaryReader = new BinaryReader(files.OpenReadStream()))
                {
                    if (loopCount == 0)
                    {
                        post.Image0 = binaryReader.ReadBytes((int)files.Length);
                    }
                    if (loopCount == 1)
                    {
                        post.Image1 = binaryReader.ReadBytes((int)files.Length);
                    }
                    if (loopCount == 2)
                    {
                        post.Image2 = binaryReader.ReadBytes((int)files.Length);
                    }
                    else if (loopCount == 3)
                    {
                        break;
                    }
                }
                loopCount++;
            }

            // Validtation (Clear > Validate > set/rerturn)
            ModelState.ClearValidationState(nameof(model.Post));

            if (post.Title == "" && post.Description == "")
            {
                ModelState.AddModelError("Post.Title", "Please enter in either a Title or a Description");
            }
            if (post.Title == "" && post.Image0 == Array.Empty<byte>())
            {
                ModelState.AddModelError("Post.Image0", "Please enter in either a Title or a Photo");
            }
            if (post.Description == "" && post.Image0 == Array.Empty<byte>())
            {
                ModelState.AddModelError("Post.Image0", "Please enter in either a Description or a Photo");
            }

            model.Post = post;

            if (!TryValidateModel(model.Post, nameof(model.Post)))
            {
                model.StepCounter = 1;
                return View("~/Views/User/CreatePost.cshtml", model);
            }

            model.ViewModelImage0 = model.Post.Image0 != null ? Convert.ToBase64String(model.Post.Image0) : null;
            model.ViewModelImage1 = model.Post.Image1 != null ? Convert.ToBase64String(model.Post.Image1) : null;
            model.ViewModelImage2 = model.Post.Image2 != null ? Convert.ToBase64String(model.Post.Image2) : null;

            return View("~/Views/User/CreatePost.cshtml", model);
        }

        [HttpGet]
        public IActionResult GoBackInPost()
        {
            PostViewModel model = new PostViewModel();

            return View("~/Views/User/CreatePost.cshtml", model);
        }
       


    }
}
