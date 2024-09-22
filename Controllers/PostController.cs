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

            model = ReturnModelImages(model);

            if (post.Title != string.Empty)
            {
                HttpContext.Session.SetString("Title", model.Post.Title);
            }
            if (post.Description != string.Empty)
            {
                HttpContext.Session.SetString("Description", model.Post.Description);
            }
            if (post.Image0 != Array.Empty<byte>())
            {
                HttpContext.Session.SetString("Image0", model.ViewModelImage0);
            }
            if (post.Image1 != Array.Empty<byte>())
            {
                HttpContext.Session.SetString("Image1", model.ViewModelImage1);
            }
            if (post.Image2 != Array.Empty<byte>())
            {
                HttpContext.Session.SetString("Image2", model.ViewModelImage2);
            }

            return View("~/Views/User/CreatePost.cshtml", model);
        }

        [HttpPost]
        [Route("Post/PostConfirm")]
        public async Task<IActionResult> PostConfirm(PostViewModel model)
        {
            model.StepCounter = 3;
            Post post = new Post();

            if (HttpContext.Session.GetString("Title") != null)
            {
                model.Post.Title = HttpContext.Session.GetString("Title");
            }
            if (HttpContext.Session.GetString("Description") != null)
            {
                model.Post.Description = HttpContext.Session.GetString("Description");
            }
            if (HttpContext.Session.GetString("Image0") != null)
            {
                model.Post.Image0 = Convert.FromBase64String(HttpContext.Session.GetString("Image0"));
            }
            if (HttpContext.Session.GetString("Image1") != null)
            {
                model.Post.Image1 = Convert.FromBase64String(HttpContext.Session.GetString("Image1"));
            }
            if (HttpContext.Session.GetString("Image2") != null)
            {
                model.Post.Image2 = Convert.FromBase64String(HttpContext.Session.GetString("Image2"));
            }

            model.Post.UserID = (int)HttpContext.Session.GetInt32("UserID");
            post = model.Post;
            post.PostDate = DateTime.Now;

            _dbContext.Add(post);
            await _dbContext.SaveChangesAsync();

            model = ReturnModelImages(model);

            HttpContext.Session.SetString("Title", string.Empty);
            HttpContext.Session.SetString("Description", string.Empty);
            HttpContext.Session.SetString("Image0", string.Empty);
            HttpContext.Session.SetString("Image1", string.Empty);
            HttpContext.Session.SetString("Image2", string.Empty);

            return View("~/Views/User/CreatePost.cshtml", model);
        }

        [HttpGet]
        public IActionResult GoBackInPost(int page)
        {
            PostViewModel model = new PostViewModel();

            page = page--;
            model.StepCounter = page;

            return View("~/Views/User/CreatePost.cshtml", model);
        }
        
        public PostViewModel ReturnModelImages(PostViewModel model)
        {
            model.ViewModelImageVariable = model.Post.Image0 != null ? Convert.ToBase64String(model.Post.Image0) : null;
            model.ViewModelImage0 = model.Post.Image0 != null ? Convert.ToBase64String(model.Post.Image0) : null;
            model.ViewModelImage1 = model.Post.Image1 != null ? Convert.ToBase64String(model.Post.Image1) : null;
            model.ViewModelImage2 = model.Post.Image2 != null ? Convert.ToBase64String(model.Post.Image2) : null;

            return (model);
        }

    }
}
