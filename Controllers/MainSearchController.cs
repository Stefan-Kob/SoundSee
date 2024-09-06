using Microsoft.AspNetCore.Mvc;
using SoundSee.Database;
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
            return View("~/Views/User/Users/AllUsers.cshtml");
        }

        // Fow now this will only return users, but eventually it needs to search posts, and hashtags
        public IActionResult ReturnSearch()
        {


            return View();
        }



    }
}
