using Microsoft.AspNetCore.Mvc;
using SoundSee.Database;
using SoundSee.Models;
using SoundSee.ViewModels;

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
        public IActionResult ReviewPost(PostViewModel model, IFormFile file)
        {
            Post post = new Post();
            int loopCount = 0;
            model.StepCounter = 2;

            post.Title = Request.Form["Title"];
            post.Description = Request.Form["Description"];
            post.Title = post.Title.Trim();
            post.Description = post.Description.Trim();

            // Collecting the 3 possible photos
            if (file != null && file.Length > 0)
            {
                for (int i = 0; i < file.Length; i++)
                {
                    using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                    {
                        if (i == 0) { post.Image0 = binaryReader.ReadBytes((int)file.Length); }
                        if (i == 1) { post.Image1 = binaryReader.ReadBytes((int)file.Length); }
                        if (i == 2) { post.Image2 = binaryReader.ReadBytes((int)file.Length); }
                    }
                    i++;
                    if (i >= 3) break;
                }
            }

            // Validtation (Clear > Validate > set/rerturn)
            ModelState.ClearValidationState(nameof(model.Post));

            if (post.Title == null && post.Description == null)
            {
                ModelState.AddModelError("Post.Title", "Please enter in either a Title or a Description");

            }
            if (post.Title == null && post.Image0 == null)
            {
                ModelState.AddModelError("Post.Image0", "Please enter in either a Title or a Photo");
            }
            if (post.Description == null && post.Image0 == null)
            {
                ModelState.AddModelError("Post.Image0", "Please enter in either a Description or a Photo");
            }

            model.Post = post;

            if (!TryValidateModel(model.Post, nameof(model.Post)))
            {
                model.StepCounter = 1;
                return View("~/Views/User/CreatePost.cshtml", model);
            }


            return View("~/Views/User/CreatePost.cshtml", model);
        }

        [HttpGet]
        public IActionResult GoBackInPost()
        {
            PostViewModel model = new PostViewModel();

            return View("~/Views/User/CreatePost.cshtml", model);
        }

        [HttpGet]
        public IActionResult ShowUserImage(PostViewModel model, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    if (model.Post.Image0 == null)
                    {
                        model.Post.Image0 = binaryReader.ReadBytes((int)file.Length);
                    }
                    if (model.Post.Image1 == null && model.Post.Image0 != null)
                    {
                        model.Post.Image1 = binaryReader.ReadBytes((int)file.Length);
                    }
                    if (model.Post.Image2 == null && model.Post.Image1 != null)
                    {
                        model.Post.Image2 = binaryReader.ReadBytes((int)file.Length);
                    }
                }
            }

            return View("~/Views/User/CreatePost.cshtml",  model);
        }

        [HttpGet]
        public IActionResult DisplayPostPhoto(PostViewModel model, IFormFile file)
        {

            return View();
        }
    }
}
