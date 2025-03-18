using Car_Rental_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Car> Cars { get; set; } = default!;
        public DbSet<Brand> Brands { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Payment> Payments { get; set; } = default!;
        public DbSet<CarRental> CarRentals { get; set; } = default!;
        public DbSet<Images> Images { get; set; } = default!;
        public DbSet<Maintenance> Maintenances { get; set; } = default!;
        public DbSet<Profile> Profiles { get; set; } = default!;
        public DbSet<Promotion> Promotions { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;
        public DbSet<ViolationHistory> ViolationHistories { get; set; } = default!;
        public DbSet<Contract> Contracts { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
