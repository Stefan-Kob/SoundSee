using SoundSee.Models;

namespace SoundSee.ViewModels
{
    public class NotificationViewModel
    {
        public User ReqUser { get; set; } = new User();

        // 0 = default, 1 = follow req, 2 = liked post, 3 = comment on your post...
        public int NotificationType { get; set; } = 0;
        public string NotificationMsg { get; set; } = string.Empty;

    }
}
