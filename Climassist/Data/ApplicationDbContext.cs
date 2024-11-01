using Microsoft.EntityFrameworkCore;
using Climassist.Models;

namespace Climassist.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // Kullanıcılar için DbSet
        public DbSet<Requests> Requests { get; set; } // Talepler için DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Gerekirse model yapılandırmalarını burada yapabilirsin
        }
    }
}
