using SoundSee.Models;

namespace SoundSee.ViewModels
{
    public class PostNUserViewModel
    {
        public UserViewModel UserVM { get; set; } = new UserViewModel();

        public PostViewModel PostVM { get; set; } = new PostViewModel();

        public List<PostViewModel> PostVMList { get; set; } = new List<PostViewModel>();

        public string SearchQuest {  get; set; } = string.Empty;

    }
}
