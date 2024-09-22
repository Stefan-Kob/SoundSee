using Microsoft.AspNetCore.Mvc;
using SoundSee.Database;
using SoundSee.Models;
using SoundSee.ViewModels;

namespace SoundSee.Controllers
{
    public class MassPostController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SoundSeeDbContext _dbContext;

        // Initialization
        public MassPostController(IWebHostEnvironment hostingEnvironment, SoundSeeDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;

        }

        [HttpGet]
        public ActionResult DisplayProfileFeed(int userId, int type)
        {
            var model = new PostNUserViewModel();
            model.UserVM = new UserViewModel();
            model.UserVM.User = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            // Get all of the users posts, and put them in the list
            foreach (Post post in _dbContext.Posts)
            {
                if (post.UserID == userId)
                {
                    PostViewModel postModel = new PostViewModel();
                    postModel.ViewModelImageVariable = post.Image0 != null ? Convert.ToBase64String(post.Image0) : null;
                    postModel.ViewModelImage0 = post.Image0 != null ? Convert.ToBase64String(post.Image0) : null;
                    postModel.ViewModelImage1 = post.Image1 != null ? Convert.ToBase64String(post.Image1) : null;
                    postModel.ViewModelImage2 = post.Image2 != null ? Convert.ToBase64String(post.Image2) : null;
                    postModel.Post = post;

                    model.PostVMList.Add(postModel);
                }
            }
            if (type == 1)
            {
                return View("~/Views/User/Users/SelectedUserFeed.cshtml", model);
            }
            else
            {
                return View("~/Views/User/UserFeedPage.cshtml", model);
            }
        }

    }
}



// Need to create DisplayUserFeed   - Will be used to see individuals feeds

// Need to create DisplayMainFeed   - Used to display the main explore page

