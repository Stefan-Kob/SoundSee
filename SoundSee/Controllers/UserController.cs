using SoundSee.Models;
using SoundSee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SoundSee.Controllers
{
    public class UserController : Controller
    {

        [HttpGet]
        [Route("User/AddUser")]
        public ActionResult AddUser()
        {
            var user = new User();
            var accounts = new List<Accounts>();
            var viewmodel = new UserViewModel { User = user, Accounts = accounts };


            return View(viewmodel);
        }


        [HttpPost]
        [Route("User/AddUser")]
        public ActionResult AddUsesr(UserViewModel model)
        {

            User user = new User();

            user.Username = Request.Form["Username"];
            user.Password = Request.Form["Password"];
            user.Email = Request.Form["Email"];
            user.SignUpForNewsletters = Request.Form["SignUpForNewsletters"];

            model.User = user;


            return View("UserAccountConfirm", model);
        }

        [HttpGet]
        [Route("User/UserAccountConfirm")]
        public ActionResult UserAccountConfirm(UserViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [Route("User/EditUser")]
        public ActionResult EditUser(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                // Update user in the database or perform other actions
                return RedirectToAction("EditUser", model);
            }

            return View(model);
        }



    }
}