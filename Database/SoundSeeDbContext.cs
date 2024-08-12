using Microsoft.EntityFrameworkCore;
using SoundSee.Models;

namespace SoundSee.Database
{
    public class SoundSeeDbContext : DbContext
    {
        public SoundSeeDbContext(DbContextOptions<SoundSeeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Accounts> Accounts { get; set; }

    }
}
