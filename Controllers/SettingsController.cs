using Microsoft.AspNetCore.Mvc;
using SoundSee.Database;
using SoundSee.Models;
using SoundSee.ViewModels;

namespace SoundSee.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SoundSeeDbContext _dbContext;

        public SettingsController(IWebHostEnvironment hostingEnvironment, SoundSeeDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;
        }

        public ActionResult SeeMainSettingsPage()
        {
            return View("~/Views/Settings/MainSettingsPage.cshtml");
        }

        public IActionResult SeeAccountInfo()
        {
            PostNUserViewModel model = new PostNUserViewModel();
            model.UserVM.User = _dbContext.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetInt32("UserID"));

            return View("~/Views/Settings/AccountInfo.cshtml", model);
        }

    }
}
