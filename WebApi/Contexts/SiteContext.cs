using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            var converter = new ValueConverter<string[],string>(
                model => string.Join(',',model),
                data => data.Split(',',StringSplitOptions.None)
            );
            //转换隐藏的文件夹
            modelBuilder.Entity<Site>().Property("HiddenDrectory").HasConversion(converter);
        }
    }
}