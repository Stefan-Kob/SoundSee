using Microsoft.EntityFrameworkCore;
using SoundSee.Models;

namespace SoundSee.Database
{
    public class SoundSeeDbContext : DbContext
    {
        public SoundSeeDbContext(DbContextOptions<SoundSeeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Accounts> Accounts { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<ChatInfoPointer> ChatInfoPointers { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<FollowRequests> FollowRequests { get; set; }
    }
}
