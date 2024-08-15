using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundSee.Database;
using SoundSee.Models;
using SoundSee.ViewModels;
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
            return View("UserSignIn");
        }

        [HttpGet]
        [Route("User/AddUser")]
        public IActionResult AddUser()
        {
            var user = new User();
            var accounts = new List<Accounts>();
            var viewmodel = new UserViewModel { User = user, Accounts = accounts };
            HttpContext.Session.SetString("User", string.Empty);

            return View("AddUser", viewmodel);
        }

        // Method for adding or editing a user. Confirms the detials and then sends the use to confirm their details
        [HttpPost]
        [Route("User/UserAccountConfirm")]
        public async Task<IActionResult> UserAccountConfirm(UserViewModel model, IFormFile file)
        {
            saltAndShaker saltAndShaker = new saltAndShaker();
            User user = new User();

            user.Username = Request.Form["Username"];
            user.Password = Request.Form["Password"];
            user.Email = Request.Form["Email"];

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

            model.User = user;

            // Validtation (Clear > Validate > set/rerturn)
            ModelState.ClearValidationState(nameof(model.User));

            foreach (var dBUser in _dbContext.Users)
            {
                if (model.User.Email != null && model.User.Email == dBUser.Email)
                {
                    ModelState.AddModelError("User.Email",
                                             "Email already in use, please use a different email");
                }
            }

            if (!TryValidateModel(model.User, nameof(model.User)))
            {
                return View("AddUser", model);
            }

            // Salting password
            string saltPassword = saltAndShaker.HashPasword(user.Password, out var salt);
            user.salt = salt;
            model.User.Password = saltPassword;

            // User data is safe after this point
            HttpContext.Session.Set("UserPhoto", model.User.Profile_Photo);

            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();


            return View("UserAccountConfirm", model);

        }

        [HttpPost]
        [Route("User/EditUser")]
        public IActionResult EditUser(UserViewModel model)
        {
            return View("EditUser", model);
        }

        [HttpPost]
        [Route("User/Welcome")]
        public IActionResult ContinUserToWelcome(UserViewModel model)
        {
            return View("Welcome", model);
        }

        public IActionResult ViewAccount()
        {
            var model = new UserViewModel();
            model.User = new User { Username = HttpContext.Session.GetString("User") };

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

    }
}
