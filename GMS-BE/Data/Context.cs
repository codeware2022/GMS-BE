using GMS_BE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GMS_BE.Data
{
    public class Context : IdentityDbContext<User>
    {
          public Context(DbContextOptions<Context> options) :base(options)
          {
            
          }
    }
}