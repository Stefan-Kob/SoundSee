using SoundSee.Models;

namespace SoundSee.ViewModels
{
    public class PostNUserViewModel
    {
        public UserViewModel UserVM { get; set; } = new UserViewModel();

        public PostViewModel PostVM { get; set; } = new PostViewModel();

        public NotificationViewModel NotificationVM { get; set; } = new NotificationViewModel();

        public List<NotificationViewModel> NotificationVMList { get; set; } = new List<NotificationViewModel>();

        public List<FollowList> FolllowListList { get; set; } = new List<FollowList> { };

        public List<PostViewModel> PostVMList { get; set; } = new List<PostViewModel>();

        public string SearchQuest { get; set; } = string.Empty;

        public string Requested { get; set; } = string.Empty;
    }
}
