using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data
{
    // Identity kullanıyorsanız IdentityDbContext<AppUser> kullanımı uygundur.
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {            
        }

        public DbSet<FcmToken> FcmTokens => Set<FcmToken>();
    }
}
