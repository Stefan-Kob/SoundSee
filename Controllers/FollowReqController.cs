using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SoundSee.Database;
using SoundSee.Models;
using SoundSee.ViewModels;

namespace SoundSee.Controllers
{
    public class FollowReqController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SoundSeeDbContext _dbContext;

        // Initialization
        public FollowReqController(IWebHostEnvironment hostingEnvironment, SoundSeeDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> ProcessFollowReq(int selectedId)
        {
            MainSearchController mainSearchController = new MainSearchController(_hostingEnvironment, _dbContext);
            PostNUserViewModel model = new PostNUserViewModel();
            FollowRequests followReq = new FollowRequests();

            model = mainSearchController.LoadingOfUser(model, 1, selectedId);
            model.Requested = "Y";

            followReq.AskingUserID = (int)HttpContext.Session.GetInt32("UserID");
            followReq.TargetUserID = model.UserVM.User.Id;

            _dbContext.Add(followReq);
            await _dbContext.SaveChangesAsync();

            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }

        public async Task<IActionResult> FollowUser(int selectedId)
        {
            MainSearchController mainSearchController = new MainSearchController(_hostingEnvironment, _dbContext);
            PostNUserViewModel model = new PostNUserViewModel();
            FollowList followList = new FollowList();

            followList.FollowedID = selectedId;
            followList.FollowerID = (int)HttpContext.Session.GetInt32("UserID");
            _dbContext.Add(followList);
            await _dbContext.SaveChangesAsync();

            model = mainSearchController.LoadingOfUser(model, 2, selectedId);

            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }

        public async Task<IActionResult> UnFollowUser(int selectedId)
        {
            MainSearchController mainSearchController = new MainSearchController(_hostingEnvironment, _dbContext);
            PostNUserViewModel model = new PostNUserViewModel();
            FollowList followList =_dbContext.FollowList.FirstOrDefault(u => u.FollowedID == selectedId && u.FollowerID == (int)HttpContext.Session.GetInt32("UserID"));

            _dbContext.Remove(followList);
            await _dbContext.SaveChangesAsync();

            model = mainSearchController.LoadingOfUser(model, 3, selectedId);


            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }
    }
}
