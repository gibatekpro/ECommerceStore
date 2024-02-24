using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Models {
    public class ProductContext : DbContext
    {



        public ProductContext(DbContextOptions<ProductContext> options) : base(options){ 

        }
        
        public DbSet<Product> Products { get; set; }


        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Address> Addresses  { get; set; }

        public DbSet<OrderItem> OrderItems  { get; set; }

        public DbSet<Order> Orders { get; set; }

    }

}