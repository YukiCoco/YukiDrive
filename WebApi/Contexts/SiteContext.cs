using Microsoft.EntityFrameworkCore;
using YukiDrive.Models;

namespace YukiDrive.Contexts
{
    public class SiteContext : DbContext
    {
        public DbSet<Site> Sites {get; set;}

        public SiteContext():base(){
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder builder){
            builder.UseSqlite(Configuration.ConnectionString);
        }
    }
}