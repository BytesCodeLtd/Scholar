using Microsoft.AspNetCore.Identity;
using Scholar.Constants;

namespace Scholar.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (string role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
