using SoundSee.Models;

namespace SoundSee.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }

        public string UserImage { get; set; } = string.Empty;

        public List<User> Users { get; set; } = new List<User>();

        public List<Accounts> Accounts { get; set; }
    }
}
