using Microsoft.AspNetCore.Identity;

namespace Scholar.Models
{
    /// <summary>
    /// A teacher or admin account. Extends Identity with app-specific profile fields.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public string? InstituteName { get; set; }
    }
}
