using Microsoft.AspNetCore.Identity;

namespace gateway.api.Persistence.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string? CreatedBy { get; set; } //This should have a value if created by a tenant and null when created by our system users
    }
}
