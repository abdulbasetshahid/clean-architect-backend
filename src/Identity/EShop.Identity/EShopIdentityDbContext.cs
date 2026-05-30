using EShop.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShop.Identity;

public class EShopIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public EShopIdentityDbContext(DbContextOptions<EShopIdentityDbContext> options) : base(options)
    {
    }
}
