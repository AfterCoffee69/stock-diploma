using Microsoft.EntityFrameworkCore;
using Stock.Models;

namespace Stock.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Delivery> Delivery { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<StockModel> Stocks { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Delivery)
                .WithOne(d => d.Order);
        }
    }
}
