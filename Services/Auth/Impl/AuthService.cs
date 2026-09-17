using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Scholar.Models;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public class AuthService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor http) : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _http = http;

        public bool IsSignedIn() => _signInManager.IsSignedIn(_http.HttpContext!.User);

        public async Task<SignInResult?> LoginAsync(LoginViewModel model)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                return null;
            }

            return await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        }

        public async Task<bool> SetPasswordUserExistsAsync(string userId)
            => await _userManager.FindByIdAsync(userId) is not null;

        public async Task<IdentityResult?> SetPasswordAsync(SetPasswordViewModel model)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null)
            {
                return null;
            }

            string token;

            try
            {
                token = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            }
            catch (FormatException)
            {
                return null;
            }

            return await _userManager.ResetPasswordAsync(user, token, model.Password);
        }

        public async Task SignOutAsync()
        {
            if (_http.HttpContext?.User.Identity?.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
            }
        }
    }
}
