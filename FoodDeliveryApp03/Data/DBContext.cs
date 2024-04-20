using FoodDeliveryApp03.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace FoodDeliveryApp03.Data
{
    public class DBContext : IdentityDbContext<IdentityUser>
    {
        public DBContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<MenuItem>MenuItems { get; set; }
        public DbSet<IdentityUser> ApplicationUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProfilePicture> ProfilePicture { get; set; }
        public DbSet<IdentityUser> Costumers { get; set; }
        public DbSet<FoodDeliveryApp03.Models.Cart> Cart { get; set; }
        public DbSet<Review> Reviews{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Order)
                .WithOne(o => o.Cart)
                .HasForeignKey<Order>(o => o.CartId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
