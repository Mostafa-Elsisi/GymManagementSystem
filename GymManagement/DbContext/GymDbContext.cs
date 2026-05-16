using GymManagement.Configurations;
using GymManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Data
{
    public class GymDbContext : DbContext
    {
        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=GymSystem;Trusted_Connection=true;TrustServerCertificate=true");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfig());
        }
        public DbSet<Plan> Plans { get; set; }
    }
}
