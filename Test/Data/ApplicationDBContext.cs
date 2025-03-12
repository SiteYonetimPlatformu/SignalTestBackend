using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data
{
     public class ApplicationDBContext : DbContext
     {
         public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
                : base(options)

         {

         }
     }
}
