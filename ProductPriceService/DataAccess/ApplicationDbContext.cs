using Microsoft.EntityFrameworkCore;
using ProductPriceService.Models;

namespace ProductPriceService.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
        {
        }

        public DbSet<Prices> Prices { get; set; }
    }
}
