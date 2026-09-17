using Microsoft.AspNetCore.Identity;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IAuthService
    {
        bool IsSignedIn();

        Task<SignInResult?> LoginAsync(LoginViewModel model);

        Task<bool> SetPasswordUserExistsAsync(string userId);

        Task<IdentityResult?> SetPasswordAsync(SetPasswordViewModel model);

        Task SignOutAsync();
    }
}
