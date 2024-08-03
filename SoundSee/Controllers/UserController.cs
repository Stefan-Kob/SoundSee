using SoundSee.Models;
using SoundSee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SoundSee.Controllers
{
    public class UserController : Controller
    {
        [Route("User/AddUser")]
        public ActionResult AddUser()
        {
            var user = new User() { Name = "Stefan", Email = "stefan@gmail.com", Id = 1};
            var accounts = new List<Accounts>
            {
                new Accounts { Name = "Account 1"},
                new Accounts { Name = "Account 2"}
            };

            var viewmodel = new UserViewModel { User = user, Accounts = accounts };

            return View(viewmodel);
        }

        
    }
}