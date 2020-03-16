using Microsoft.EntityFrameworkCore;
using YukiDrive.Models;
namespace YukiDrive.Contexts
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UserContext() : base()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite(Configuration.ConnectionString);
        }
    }
}