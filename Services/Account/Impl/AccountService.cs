using Microsoft.AspNetCore.Identity;
using Scholar.Models;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public class AccountService(UserManager<ApplicationUser> userManager, IHttpContextAccessor http) : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _http = http;

        public async Task<AccountViewModel?> GetProfileAsync()
        {
            ApplicationUser? user = await CurrentUserAsync();

            if (user is null)
            {
                return null;
            }

            return new AccountViewModel
            {
                FullName = user.FullName,
                Email = user.Email
            };
        }

        public async Task<bool> UpdateProfileAsync(AccountViewModel model)
        {
            ApplicationUser? user = await CurrentUserAsync();

            if (user is null)
            {
                return false;
            }

            string email = model.Email!.Trim();

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                ApplicationUser? existing = await _userManager.FindByEmailAsync(email);

                if (existing is not null && existing.Id != user.Id)
                {
                    return false;
                }

                IdentityResult emailResult = await _userManager.SetEmailAsync(user, email);
                IdentityResult userNameResult = await _userManager.SetUserNameAsync(user, email);

                if (!emailResult.Succeeded || !userNameResult.Succeeded)
                {
                    return false;
                }

                user.EmailConfirmed = true;
            }

            user.FullName = model.FullName!.Trim();

            IdentityResult result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        private Task<ApplicationUser?> CurrentUserAsync() => _userManager.GetUserAsync(_http.HttpContext!.User);
    }
}
