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

            return identity;
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static int? GetInstituteId(this ClaimsPrincipal principal)
            => int.TryParse(principal.FindFirstValue(AppClaims.InstituteId), out int id) ? id : null;
    }
}
