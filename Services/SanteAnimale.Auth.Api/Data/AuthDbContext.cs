using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SanteAnimale.Auth.Api.Entities;

namespace SanteAnimale.Auth.Api.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
    }
}
