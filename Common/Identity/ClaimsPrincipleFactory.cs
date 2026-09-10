using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Scholar.Constants;
using Scholar.Models;

namespace Scholar.Common.Identity
{
    public class ClaimsPrincipleFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ClaimsPrincipleFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            if (user.InstituteId is int instituteId)
            {
                identity.AddClaim(new Claim(AppClaims.InstituteId, instituteId.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                identity.AddClaim(new Claim(AppClaims.FullName, user.FullName));
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                identity.AddClaim(new Claim(AppClaims.Email, user.Email));
            }

            return identity;
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static int? GetInstituteId(this ClaimsPrincipal principal)
            => int.TryParse(principal.FindFirstValue(AppClaims.InstituteId), out int id) ? id : null;

        public static string GetDisplayName(this ClaimsPrincipal principal)
        {
            string? name = principal.FindFirstValue(AppClaims.FullName);

            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return principal.GetEmail() ?? principal.Identity?.Name ?? "User";
        }

        public static string? GetEmail(this ClaimsPrincipal principal)
            => principal.FindFirstValue(AppClaims.Email);
    }
}
