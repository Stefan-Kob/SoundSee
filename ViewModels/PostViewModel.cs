using SoundSee.Models;
using System.Buffers.Text;

namespace SoundSee.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; }

        public string ViewModelImage0 { get; set; } = string.Empty;

        public string ViewModelImage1 { get; set; } = string.Empty;

        public string ViewModelImage2 { get; set; } = string.Empty;

        public User PostUser { get; set; }

        public int StepCounter { get; set; } = 0;
    }
}
