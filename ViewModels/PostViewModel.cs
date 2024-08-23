using SoundSee.Models;

namespace SoundSee.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; }

        public User PostUser { get; set; }

        public int StepCounter { get; set; } = 0;
    }
}
