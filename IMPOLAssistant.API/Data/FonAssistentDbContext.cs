using Microsoft.EntityFrameworkCore;
using FONAssistant.Shared.Models;
using System;

namespace FONAssistant.API.Data
{
    public class FonAssistentDbContext:DbContext
    {
        public FonAssistentDbContext(DbContextOptions<FonAssistentDbContext> options) : base(options)
        {
        }

        public DbSet<Predmet> Predmeti { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Predmet>()
                .HasIndex(p => p.Sifra)
                .IsUnique();

            modelBuilder.Entity<User>()
                 .HasIndex(u => u.Username)
                 .IsUnique(); 
        }
    }
}
