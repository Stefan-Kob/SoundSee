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

            model = mainSearchController.LoadingOfUser(model, 1, selectedId);
            model.Requested = "Y";

            FollowRequests followReq = new FollowRequests();
            followReq.AskingUserID = (int)HttpContext.Session.GetInt32("UserID");
            followReq.TargetUserID = model.UserVM.User.Id;

            _dbContext.Add(followReq);
            await _dbContext.SaveChangesAsync();

            return View("~/Views/User/Users/SelectedUser.cshtml", model);
        }

    }
}
