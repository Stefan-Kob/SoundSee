using SoundSee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoundSee.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }
        public List<Accounts> Accounts { get; set; }
    }
}