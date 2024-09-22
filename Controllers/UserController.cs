using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundSee.Database;
using SoundSee.Models;
using SoundSee.ViewModels;
using System.Diagnostics;
using System.IO;

namespace SoundSee.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SoundSeeDbContext _dbContext;

        // Initialization
        public UserController(IWebHostEnvironment hostingEnvironment, SoundSeeDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;

        }

        public IActionResult SendToSignIn()
        {
            if (CheckIfSignedIn())
            {
                PostNUserViewModel model = new PostNUserViewModel();
                model.PostVMList = PostVMListFP(model, HttpContext.Session.GetInt32("UserID"));
                return View("Welcome", model);
            }

            return View("~/Views/SignIn/UserSignIn.cshtml");
        }

        // From sign in page, validate proper sign in, then go into the app
        [HttpPost]
        public IActionResult SignIn(UserViewModel userModel)
        {
            PostNUserViewModel model = new PostNUserViewModel();
            model.UserVM = new UserViewModel();
            model.UserVM = userModel;

            User user = new User();
            User tempUser = new User();
            saltAndShaker saltAndShaker = new saltAndShaker();
            int inDBCheck = 0;

            user.Email = Request.Form["Email"];
            user.Password = Request.Form["Password"];

            // Validtation (Clear > Validate > set/rerturn)
            ModelState.ClearValidationState(nameof(model.UserVM.User));

            foreach (var dBUser in _dbContext.Users)
            {
                bool correctPassword = true;

                if (dBUser.Email == user.Email)
                {
                    tempUser.Password = dBUser.Password;
                    tempUser.Salt = dBUser.Salt;
                    user.Email = dBUser.Email;
                    user.Username = dBUser.Username;
                    user.Profile_Photo = dBUser.Profile_Photo;
                    user.Id = dBUser.Id;
                    inDBCheck = 1;

                    correctPassword = saltAndShaker.VerifyPassword(user.Password, tempUser.Password, tempUser.Salt);
                    if (correctPassword == false)
                    {
                        ModelState.AddModelError("User.Password", "Wrong password, please try again.");
                    }
                    break;
                }
            }
                
            if (inDBCheck == 0)
            {
                ModelState.AddModelError("User.Email", "Wrong email or password, please try again.");
                ModelState.AddModelError("User.Password", "Wrong email or password, please try again.");
            }

            model.UserVM.User = user;
            if (!TryValidateModel(model.UserVM.User, nameof(model.UserVM.User)))
            {
                return View("~/Views/SignIn/UserSignIn.cshtml", model);
            }

            model.UserVM.User.Password = string.Empty;
            HttpContext.Session.Set("UserPhoto", model.UserVM.User.Profile_Photo);
            HttpContext.Session.SetString("User", model.UserVM.User.Username);
            HttpContext.Session.SetString("UserEmail", model.UserVM.User.Email);
            HttpContext.Session.SetInt32("UserID", model.UserVM.User.Id);

            model.PostVMList = PostVMListFP(model, HttpContext.Session.GetInt32("UserID"));
            return View("Welcome", model);
        }

        // Send user to create an account
        [HttpGet]
        [Route("SignIn/AddUser")]
        public IActionResult AddUser()
        {
            if (CheckIfSignedIn())
            {
                PostNUserViewModel model = new PostNUserViewModel();
                model.PostVMList = PostVMListFP(model, HttpContext.Session.GetInt32("UserID"));
                return View("~/Views/User/Welcome.cshtml", model);
            }

            var user = new User();
            var accounts = new List<Accounts>();
            var viewmodel = new UserViewModel { User = user, Accounts = accounts };

            return View("~/Views/SignIn/AddUser.cshtml", viewmodel);
        }

        // Method for adding or editing a user. Confirms the detials and then sends the use to confirm their details
        [HttpPost]
        [Route("SignIn/UserAccountConfirm")]
        public async Task<IActionResult> UserAccountConfirm(UserViewModel model, IFormFile file)
        {
            User user = new User();
            saltAndShaker saltAndShaker = new saltAndShaker();

            user.Username = Request.Form["Username"];
            user.Password = Request.Form["Password"];
            user.Email = Request.Form["Email"];
            user.Username = user.Username.Trim();
            user.Password = user.Password.Trim();


            if (file != null && file.Length > 0)
            {
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    user.Profile_Photo = binaryReader.ReadBytes((int)file.Length);
                }
            }
            else
            {
                string path = Path.Combine(_hostingEnvironment.WebRootPath, "Media", "defaultProfile_Photo.jpg");
                byte[] default_Photo_Byte = System.IO.File.ReadAllBytes(path);

                user.Profile_Photo = default_Photo_Byte;
            }

            if (Request.Form["SignUpForNewsletters"] != "on")
            {
                user.SignUpForNewsletters = "F";
            }
            else
            {
                user.SignUpForNewsletters = "T";
            }

            if (Request.Form["PublicOrPrivateAcc"] != "on")
            {
                user.PublicOrPrivateAcc = "Public";
            }
            else
            {
                user.PublicOrPrivateAcc = "Private";
            }

            if (Request.Form["ShowInDiscover"] != "on")
            {
                user.ShowInDiscover = "N";
            }
            else
            {
                user.ShowInDiscover = "Y";
            }
            model.User = user;

            // Validtation (Clear > Validate > set/rerturn)
            ModelState.ClearValidationState(nameof(model.User));

            User dBUser = _dbContext.Users.FirstOrDefault(u => u.Email == model.User.Email);
            if (dBUser != null && !string.IsNullOrEmpty(dBUser.Email))
            {
                if (model.User.Email != null && model.User.Email == dBUser.Email)
                {
                    ModelState.AddModelError("User.Email", "Email already in use, please use a different email");
                }
            }

            if (!TryValidateModel(model.User, nameof(model.User)))
            {
                return View("~/Views/SignIn/AddUser.cshtml", model);
            }

            // Salting password
            string saltPassword = saltAndShaker.HashPasword(user.Password, out var salt);
            user.Salt = salt;
            model.User.Password = saltPassword;

            // User data is safe after this point
            HttpContext.Session.Set("UserPhoto", model.User.Profile_Photo);
            HttpContext.Session.SetString("User", model.User.Username);
            HttpContext.Session.SetString("UserEmail", model.User.Email);

            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();

            User userForID = _dbContext.Users.FirstOrDefault(u => u.Email == model.User.Email);
            HttpContext.Session.SetInt32("UserID", userForID.Id);

            return View("~/Views/SignIn/UserAccountConfirm.cshtml", model);

        }

        [HttpPost]
        [Route("SignIn/EditUser")]
        public IActionResult EditUser(UserViewModel model)
        {
            return View("~/Views/SignIn/EditUser.cshtml", model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        public IActionResult ContinUserToWelcome(PostNUserViewModel model)
        {
            model.PostVMList = PostVMListFP(model, HttpContext.Session.GetInt32("UserID"));
            return View("~/Views/User/Welcome.cshtml", model);
        }

        public IActionResult ViewAccount()
        {
            PostNUserViewModel model = new PostNUserViewModel();
            int? userID = HttpContext.Session.GetInt32("UserID");
            model = GetUserAccountToDisplay(model, userID);

            return View("ViewAccount", model);
        }

        public IActionResult DisplayProfilePhoto()
        {
            var user = new User { Profile_Photo = HttpContext.Session.Get("UserPhoto") };

            if (user.Profile_Photo != null)
            {
                return File(user.Profile_Photo, "image/jpeg");
            }
            else
            {
                string path = Path.Combine(_hostingEnvironment.WebRootPath, "Media", "defaultProfile_Photo.jpg");
                byte[] default_Photo_Byte = System.IO.File.ReadAllBytes(path);

                return File(default_Photo_Byte, "image/jpeg");
            }
        }

        public bool CheckIfSignedIn()
        {
            User user = new User();
            UserViewModel model = new UserViewModel();

            // Send user to sign in if already signed in
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                foreach (var dBUser in _dbContext.Users)
                {
                    if (dBUser.Email == HttpContext.Session.GetString("UserEmail") && dBUser.Username == HttpContext.Session.GetString("User"))
                    {
                        user.Email = dBUser.Email;
                        user.Username = dBUser.Username;
                        model.User = user;
                        HttpContext.Session.SetString("User", model.User.Username);
                        HttpContext.Session.SetString("UserEmail", model.User.Email);
                        HttpContext.Session.SetInt32("UserID", model.User.Id);

                        return true;
                    }
                }
            }

            return false;
        }

        public List<NotificationViewModel> GetNotificationList(PostNUserViewModel model)
        {
            // Get all of the users notifications, and put them in the ViewModel list. Will include more notifications in the future
            foreach (FollowRequests followReq in _dbContext.FollowRequests)
            {
                NotificationViewModel NotifModel = new NotificationViewModel();
                if (followReq.TargetUserID == model.UserVM.User.Id)
                {
                    NotifModel.ReqUser = _dbContext.Users.FirstOrDefault(u => u.Id == followReq.AskingUserID);
                    NotifModel.ReqUser.UserImage = NotifModel.ReqUser.Profile_Photo != null ? Convert.ToBase64String(NotifModel.ReqUser.Profile_Photo) : null;
                    NotifModel.ID = followReq.RequestID;
                    model.NotificationVMList.Add(NotifModel);
                }
            }

            model.NotificationVMList = model.NotificationVMList.OrderByDescending(n => n.ID).ToList();

            return model.NotificationVMList;
        }

        public List<PostViewModel> GetPostList(PostNUserViewModel model)
        {
            // Get all of the users posts, and put them in the list
            foreach (Post post in _dbContext.Posts)
            {
                if (post != null)
                {
                    PostViewModel postModel = new PostViewModel();
                    if (post.UserID == model.UserVM.User.Id)
                    {
                        postModel.ViewModelImageVariable = post.Image0 != null ? Convert.ToBase64String(post.Image0) : null;
                        postModel.ViewModelImage0 = post.Image0 != null ? Convert.ToBase64String(post.Image0) : null;
                        postModel.ViewModelImage1 = post.Image1 != null ? Convert.ToBase64String(post.Image1) : null;
                        postModel.ViewModelImage1 = post.Image2 != null ? Convert.ToBase64String(post.Image1) : null;
                        postModel.Post = post;

                        if (post.Title.Count() > 23)
                        {
                            postModel.Post.Title = post.Title.Substring(0, 23) + "...";
                        }
                        if (post.Description.Count() > 80)
                        {
                            postModel.Post.Description = post.Description.Substring(0, 100) + "...";
                        }

                        model.PostVMList.Add(postModel);
                    }
                } 
            }

            model.PostVMList = model.PostVMList.OrderByDescending(n => n.Post.Id).ToList();

            return model.PostVMList;
        }

        // Returns a list of posts (users welcome page)
        public List<PostViewModel> PostVMListFP(PostNUserViewModel model, int? userId)
        {
            // Get the list of ID's of people the user follow (.followedID)
            foreach (FollowList followList in _dbContext.FollowList)
            {
                if (followList.FollowerID == userId)
                {
                    model.FolllowListList.Add(followList);
                }
            }

            // Get all of the users posts, and put them in the list
            if (model.FolllowListList != null)
            {
                foreach (Post post in _dbContext.Posts)
                {
                    if (post != null)
                    {
                        foreach (FollowList follow in model.FolllowListList)
                        {
                            if (post.UserID == follow.FollowedID)
                            {
                                PostViewModel postModel = new PostViewModel();
                                postModel.ViewModelImageVariable = post.Image0 != null ? Convert.ToBase64String(post.Image0) : null;
                                postModel.ViewModelImage0 = post.Image0 != null ? Convert.ToBase64String(post.Image0) : null;
                                postModel.ViewModelImage1 = post.Image1 != null ? Convert.ToBase64String(post.Image1) : null;
                                postModel.ViewModelImage1 = post.Image2 != null ? Convert.ToBase64String(post.Image1) : null;
                                postModel.Post = post;

                                model.PostVMList.Add(postModel);
                            }
                        }
                    }
                }
            }

            model.PostVMList = model.PostVMList.OrderByDescending(n => n.Post.Id).ToList();

            return model.PostVMList;
        }

        public PostNUserViewModel GetUserAccountToDisplay(PostNUserViewModel model, int? userID)
        {
            model.UserVM = new UserViewModel();
            model.NotificationVMList = new List<NotificationViewModel>();
            model.UserVM.User = _dbContext.Users.FirstOrDefault(u => u.Id == userID);

            model.PostVMList = GetPostList(model);
            model.NotificationVMList = GetNotificationList(model);

            return model;
        }

    }
}
