using Microsoft.EntityFrameworkCore;
using ProductAPIDomain.Entities;

namespace ProductAPIInfraestructure.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
