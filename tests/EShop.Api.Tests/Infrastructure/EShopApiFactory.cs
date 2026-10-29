using EShop.Application.Contracts;
using EShop.Domain.Entities;
using EShop.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EShop.Api.Tests.Infrastructure;

public sealed class EShopApiFactory : WebApplicationFactory<Program>
{
    public static readonly Guid VariantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid InsideDhakaId = Guid.Parse("8f3c1a2e-4b6d-4e91-9c07-1a2b3c4d5e6f");
    public const decimal VariantPrice = 500m;
    public const decimal InsideDhakaCost = 80m;

    private readonly string _databaseName = $"eshop-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Firebase:ProjectId"] = "eshop-test-project"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<EShopDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<EShopDbContext>>();
            services.AddDbContext<EShopDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            services.RemoveAll<IFirebaseTokenVerifier>();
            services.AddSingleton<IFirebaseTokenVerifier, FakeFirebaseTokenVerifier>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EShopDbContext>();
        db.Database.EnsureCreated();
        Seed(db);

        return host;
    }

    public T WithDb<T>(Func<EShopDbContext, T> action)
    {
        using var scope = Services.CreateScope();
        return action(scope.ServiceProvider.GetRequiredService<EShopDbContext>());
    }

    private static void Seed(EShopDbContext db)
    {
        if (db.ProductVariants.Any(v => v.Id == VariantId))
            return;

        var category = new Category { Id = Guid.NewGuid(), Name = "Test Category", CreatedBy = "test" };
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            CategoryId = category.Id,
            Category = category,
            CreatedBy = "test"
        };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.ProductVariants.Add(new ProductVariant
        {
            Id = VariantId,
            VariationName = "Default",
            Price = VariantPrice,
            InStock = true,
            IsActive = true,
            ProductId = product.Id,
            Product = product,
            CreatedBy = "test"
        });
        db.SaveChanges();
    }
}
