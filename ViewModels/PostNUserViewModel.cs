using SoundSee.Models;

namespace SoundSee.ViewModels
{
    public class PostNUserViewModel
    {
        public UserViewModel UserVM { get; set; } = new UserViewModel();

        public List<PostViewModel> PostVMList { get; set; } = new List<PostViewModel>();

    }
}
