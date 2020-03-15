using Microsoft.EntityFrameworkCore;
using YukiDrive.Models;
using System;

namespace YukiDrive.Contexts
{
    public class DriveContext:DbContext
    {
        public DbSet<DriveFile> DriveFiles { get; set; }

        public DriveContext():base(){

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder){
            builder.UseSqlite(Configuration.ConnectionString);
        }
    }
}