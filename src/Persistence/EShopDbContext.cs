using EShop.Application.Contracts;
using EShop.Domain.Common;
using EShop.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShop.Persistence
{
    public class EShopDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService? _currentUserService;

        public EShopDbContext(DbContextOptions<EShopDbContext> options) : base(options)
        {
        }

        public EShopDbContext(
            DbContextOptions<EShopDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentProvider> PaymentProviders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EShopDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            string userId = _currentUserService?.GetCurrentUserId() ?? "system";

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
