using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
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

        public async Task<IActionResult> ProcessFollowReq(int selectedId, int userID)
        {
            MainSearchController mainSearchController = new MainSearchController(_hostingEnvironment, _dbContext);
            PostNUserViewModel model = new PostNUserViewModel();
            FollowRequests followReq = new FollowRequests();

            model = mainSearchController.LoadingOfUser(model, 1, selectedId, userID);
            model.Requested = "Y";

            followReq.AskingUserID = userID;
            followReq.TargetUserID = model.UserVM.User.Id;

            _dbContext.Add(followReq);
            await _dbContext.SaveChangesAsync();

            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }

        public async Task<IActionResult> FollowUser(int selectedId, int userID)
        {
            MainSearchController mainSearchController = new MainSearchController(_hostingEnvironment, _dbContext);
            PostNUserViewModel model = new PostNUserViewModel();
            FollowList followList = new FollowList();

            followList.FollowedID = selectedId;
            followList.FollowerID = userID;
            _dbContext.Add(followList);
            await _dbContext.SaveChangesAsync();

            model = mainSearchController.LoadingOfUser(model, 2, selectedId, userID);

            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }

        public async Task<IActionResult> UnFollowUser(int selectedId, int userID)
        {
            MainSearchController mainSearchController = new MainSearchController(_hostingEnvironment, _dbContext);
            PostNUserViewModel model = new PostNUserViewModel();
            FollowList followList =_dbContext.FollowList.FirstOrDefault(u => u.FollowedID == selectedId && u.FollowerID == userID);

            _dbContext.Remove(followList);
            await _dbContext.SaveChangesAsync();

            model = mainSearchController.LoadingOfUser(model, 3, selectedId, userID);


            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }

        public async Task<IActionResult> AcceptFollowReq(int requestUserId, int userID)
        {
            UserController userController = new UserController(_hostingEnvironment, _dbContext);
            PostNUserViewModel model = new PostNUserViewModel();
            FollowList followList = new FollowList();

            FollowRequests followReq = _dbContext.FollowRequests.FirstOrDefault(u => u.TargetUserID == userID && u.AskingUserID == requestUserId);

            followList.FollowedID = userID;
            followList.FollowerID = requestUserId;

            _dbContext.Remove(followReq);
            _dbContext.Add(followList);
            await _dbContext.SaveChangesAsync();

            model = userController.GetUserAccountToDisplay(model, userID);

            return View("~/Views/User/ViewAccount.cshtml", model);
        }

        public async Task<IActionResult> DenyFollowReq()
        {

            return View();
        }
    }
}
