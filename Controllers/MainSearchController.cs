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
        private string search = string.Empty;
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
                            if (model.UserVM.Users.Count >= 10)
                            {
                                break;
                            }
                            user.UserImage = user.Profile_Photo != null ? Convert.ToBase64String(user.Profile_Photo) : null;
                            if (user.Username.Count() >= 18)
                            {
                                user.Username = user.Username.Substring(0, 18) + "...";
                            }
                            model.UserVM.Users.Add(user);
                        }
                    }
                }
                return View("~/Views/User/Users/AllUsers.cshtml", model);
            }
        }

        // Send user to see the selected accounts page
        public IActionResult LoadSelectedUser(string search)
        {
            PostNUserViewModel model = new PostNUserViewModel();
            this.search = search;
            model = LoadingOfUser(model, 0, 0);

            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }

        public PostNUserViewModel LoadingOfUser(PostNUserViewModel model, int type, int selectedId)
        {
            // Create User
            model.PostVM = new PostViewModel();

            if (type == 1 || type == 2 || type == 3 && selectedId != 0)
            {
                model.UserVM.User = _dbContext.Users.FirstOrDefault(u => u.Id == selectedId);
            }
            else
            {
                model.UserVM.User = _dbContext.Users.FirstOrDefault(u => u.Id == Convert.ToInt64(search));
            }

            model.UserVM.UserImage = model.UserVM.User.Profile_Photo != null ? Convert.ToBase64String(model.UserVM.User.Profile_Photo) : null;

            // Add users posts
            foreach (Post post in _dbContext.Posts)
            {
                PostViewModel postModel = new PostViewModel();

                if (post.UserID == model.UserVM.User.Id)
                {
                    postModel.Post = post;
                    model.PostVMList.Add(postModel);
                }
            }

            if (model.UserVM.User.PublicOrPrivateAcc == "Public")
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                if (userId == null)
                {
                    // Handle the case where the session key is not set
                    string igloo = "fuck";
                }
                FollowList followList = new FollowList();
                if (_dbContext.FollowList.Count() != 0)
                {
                    followList = _dbContext.FollowList.FirstOrDefault(u => u.FollowedID == model.UserVM.User.Id && u.FollowerID == (int)HttpContext.Session.GetInt32("UserID"));
                }
 
                if (followList != null || type == 2)
                {
                    model.Requested = "Y";
                }
                else
                {
                    model.Requested = "N";
                }
            }
            else
            {
                // See if you have followed them or not after knowing this is not a public account
                FollowRequests followReq = new FollowRequests();
                if (_dbContext.FollowRequests.Count() != 0)
                {
                    followReq = _dbContext.FollowRequests.FirstOrDefault(u => u.TargetUserID == model.UserVM.User.Id && u.AskingUserID == (int)HttpContext.Session.GetInt32("UserID"));
                }

                FollowList followList = new FollowList();
                if (_dbContext.FollowList.Count() != 0)
                {
                    followList = _dbContext.FollowList.FirstOrDefault(u => u.FollowedID == model.UserVM.User.Id && u.FollowerID == (int)HttpContext.Session.GetInt32("UserID"));
                }

                if (followList != null)
                {
                    model.Requested = "F";
                }
                if (followReq != null && followReq.AskingUserID == HttpContext.Session.GetInt32("UserID"))
                {
                    model.Requested = "Y";
                }
                else
                {
                    model.Requested = "N";
                }
                if (type == 3)
                {
                    model.Requested = "N";
                }
            }
            return model;
        }



    }
}
