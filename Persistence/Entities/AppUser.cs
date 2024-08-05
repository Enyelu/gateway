using Microsoft.AspNetCore.Identity;
using System.Net;

namespace gateway.api.Persistence.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Avatar { get; set; }
        public bool IsActive { get; set; }
        public bool IsTenantStaff { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime? RefereshTokenExpiry { get; set; }
    }
}
