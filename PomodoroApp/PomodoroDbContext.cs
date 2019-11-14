using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PomodoroApp
{
    public class PomodoroDbContext : DbContext
    {
        public DbSet<PomodoroSessionEntity> Pomodoros { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Pomodoro2019.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });


            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names
            modelBuilder.Entity<PomodoroSessionEntity>().ToTable("PomodoroSessions", "dbo");
            modelBuilder.Entity<PomodoroSessionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Id).IsUnique();
            });


            base.OnModelCreating(modelBuilder);
        }
    }
    public class PomodoroSessionEntity
    {
        public string Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int Type { get; set; }
        public string Comment { get; set; }

    }
}
