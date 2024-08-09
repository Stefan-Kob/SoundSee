using SoundSee.Models;
using SoundSee.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SoundSee.Controllers
{
    public class UserController : Controller
    {

        // Sends the user to the adduser page
        [HttpGet]
        [Route("User/AddUser")]
        public ActionResult AddUser()
        {
            var user = new User();
            var accounts = new List<Accounts>();
            var viewmodel = new UserViewModel { User = user, Accounts = accounts };
            Session["User"] = string.Empty;

            return View("AddUser", viewmodel);
        }

        // Method for getting the input info and display to user
        [HttpPost]
        [Route("User/UserAccountConfirm")]
        public ActionResult UserAccountConfirm(UserViewModel model)
        {
            User user = new User();

            // Method for selecting the profile photo
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        user.Profile_Photo = binaryReader.ReadBytes(file.ContentLength);
                    }
                }
            }

            user.Username = Request.Form["Username"];
            user.Password = Request.Form["Password"];
            user.Email = Request.Form["Email"];
            user.SignUpForNewsletters = Request.Form["SignUpForNewsletters"];

            model.User = user;
            Session["User"] = user;

            return View("UserAccountConfirm", model);
        }

        [HttpPost]
        [Route("User/EditUser")]
        public ActionResult EditUser(UserViewModel model)
        {
            return View("EditUser", model);
        }

        // Sends the user to the welcome page
        [HttpPost]
        [Route("User/Welcome")]
        public ActionResult ContinUserToWelcome(UserViewModel model)
        {

            return View("Welcome", model);
        }

        public ActionResult DisplayProfilePhoto()
        {
            var user = Session["User"] as User;

            if (user.Profile_Photo != null)
            {
                return File(user.Profile_Photo, "image/jpeg");
            }
            else
            {
                string path = Server.MapPath("~/Media/defaultProfile_Photo.jpg");
                byte[] default_Photo_Byte = System.IO.File.ReadAllBytes(path);

                return File(default_Photo_Byte, "image/jpeg");
            }
        }











        // THIS IS ONLY TEMP AND FOR DEV PURPOSES ONLY =====================*=====================*=====================
        public static Array UserArray = new Array[100];

    }
}