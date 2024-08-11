using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundSee.Models;
using SoundSee.ViewModels;
using System.IO;

namespace SoundSee.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
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

        [HttpPost]
        [Route("User/UserAccountConfirm")]
        public IActionResult UserAccountConfirm(UserViewModel model, IFormFile file)
        {
            User user = new User();

            if (file != null && file.Length > 0)
            {
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    user.Profile_Photo = binaryReader.ReadBytes((int)file.Length);
                }
            }

            user.Username = Request.Form["Username"];
            user.Password = Request.Form["Password"];
            user.Email = Request.Form["Email"];
            user.SignUpForNewsletters = Request.Form["SignUpForNewsletters"];

            model.User = user;
            HttpContext.Session.SetString("User", user.Username);

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
            var user = new User { Username = HttpContext.Session.GetString("User") };

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










        // THIS IS ONLY TEMP AND FOR DEV PURPOSES ONLY =====================*=====================*=====================
        public static Array UserArray = new Array[100];
    }
}
