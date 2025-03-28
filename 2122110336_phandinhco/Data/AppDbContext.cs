using _2122110336_phandinhco.Model;
using Microsoft.EntityFrameworkCore;

namespace _2122110336_phandinhco.Data
{
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
            public DbSet<Product> Products { get; set; }
        }
}
