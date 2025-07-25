using Microsoft.EntityFrameworkCore;
using ProductCatalogService.Models;

namespace ProductCatalogService.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToTable("Products");
        }
        public DbSet<Product> Products { get; set; }
    }
}
