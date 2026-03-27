using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class EShopDbContext : DbContext
    {
        public EShopDbContext(DbContextOptions<EShopDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
    }
}
