using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EShop.Persistence;

public class EShopDbContextFactory : IDesignTimeDbContextFactory<EShopDbContext>
{
    public EShopDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var optionsBuilder = new DbContextOptionsBuilder<EShopDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new EShopDbContext(optionsBuilder.Options);
    }

    private static IConfiguration BuildConfiguration()
    {
        var basePath = ResolveApiProjectPath();

        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();
    }

    private static string ResolveApiProjectPath()
    {
        var candidates = new[]
        {
            Directory.GetCurrentDirectory(),
            Path.Combine(Directory.GetCurrentDirectory(), "src", "Api", "EShop.Api"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "Api", "EShop.Api"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Api", "EShop.Api"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Api", "EShop.Api")
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(Path.Combine(fullPath, "appsettings.json")))
            {
                return fullPath;
            }
        }

        throw new InvalidOperationException(
            "Could not locate EShop.Api/appsettings.json for design-time DbContext configuration.");
    }
}
