using DoAn4.Models;

using Microsoft.EntityFrameworkCore;


namespace DoAn4.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Notify> Notifies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<UserOTP> UserOTPs { get; set; }
        public DbSet<Conversations> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}