using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using gateway.api.Persistence.Entities;

namespace gateway.api.Persistence.Database
{
    public class ApplicationContext : IdentityDbContext<AppUser, ApplicationRole, string>
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Staff> StaffMembers { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}
