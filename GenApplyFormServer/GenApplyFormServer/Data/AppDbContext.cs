using GenApplyFormServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GenApplyFormServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Config> Configs => Set<Config>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.VerifyCode)
                .IsUnique(); // 确保验证码唯一
        }
    }
}
