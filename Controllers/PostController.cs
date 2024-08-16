using Microsoft.AspNetCore.Mvc;

namespace SoundSee.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
