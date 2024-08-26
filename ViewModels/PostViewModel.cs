using SoundSee.Models;
using System.Buffers.Text;

namespace SoundSee.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; }
        public User PostUser { get; set; }

        public string ViewModelImageVariable { get; set; } = string.Empty;

        public string ViewModelImage0 { get; set; } = string.Empty;

        public string ViewModelImage1 { get; set; } = string.Empty;

        public string ViewModelImage2 { get; set; } = string.Empty;

        public int StepCounter { get; set; } = 0;
    }
}
