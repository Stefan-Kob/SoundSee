using Microsoft.AspNetCore.Mvc;
using SoundSee.Database;
using SoundSee.Models;
using SoundSee.ViewModels;

namespace SoundSee.Controllers
{
    public class MainSearchController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SoundSeeDbContext _dbContext;

        // Initialization
        public MainSearchController(IWebHostEnvironment hostingEnvironment, SoundSeeDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;

        }

        public IActionResult GoToSearch()
        { 
            PostNUserViewModel model = new PostNUserViewModel();

            return View("~/Views/User/Users/AllUsers.cshtml", model);
        }

        // Fow now this will only return users, but eventually it needs to search posts, and hashtags
        public IActionResult DisplaySearchResults(PostNUserViewModel model)
        {
            string searchQuest = Request.Form["SearchQuest"];
            model.UserVM.Users = [];

            if (string.IsNullOrEmpty(searchQuest))
            {
                return View("~/Views/User/Users/AllUsers.cshtml", model);
            }
            else
            {
                foreach (User user in _dbContext.Users)
                {
                    if (user.Username.StartsWith(searchQuest,StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (user.Username != HttpContext.Session.GetString("User"))
                        {
                            user.UserImage = user.Profile_Photo != null ? Convert.ToBase64String(user.Profile_Photo) : null;
                            model.UserVM.Users.Add(user);
                        }
                    }
                }
                return View("~/Views/User/Users/AllUsers.cshtml", model);
            }
        }

        public IActionResult LoadSelectedUser(string search)
        {
            PostNUserViewModel model = new PostNUserViewModel();
            model.PostVM = new PostViewModel();

            model.UserVM.User = _dbContext.Users.FirstOrDefault(u => u.Id == Convert.ToInt64(search));
            model.UserVM.UserImage = model.UserVM.User.Profile_Photo != null ? Convert.ToBase64String(model.UserVM.User.Profile_Photo) : null;

            foreach (Post post in _dbContext.Posts)
            {
                PostViewModel postModel = new PostViewModel();

                if (post.UserID == model.UserVM.User.Id)
                {
                    postModel.Post = post;
                    model.PostVMList.Add(postModel);
                }
            }

            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }




    }
}
