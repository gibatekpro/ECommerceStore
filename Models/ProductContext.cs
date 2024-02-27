using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Models {
    public class ProductContext : IdentityDbContext<IdentityUser>
    {



        public ProductContext(DbContextOptions<ProductContext> options) : base(options){ 

        }
        
        public DbSet<Product> Products { get; set; }


        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Address> Addresses  { get; set; }

        public DbSet<OrderItem> OrderItems  { get; set; }

        public DbSet<Order> Orders { get; set; }
        
        public DbSet<OrderStatus> OrderStatus { get; set; }

    }

}