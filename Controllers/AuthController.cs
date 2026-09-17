using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scholar.Constants;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    public class AuthController(IAuthService auth, ILogger<AuthController> logger) : Controller
    {
        private readonly IAuthService _auth = auth;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (_auth.IsSignedIn())
            {
                return RedirectToAction(nameof(DashboardController.Index), ControllerNames.Dashboard);
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Microsoft.AspNetCore.Identity.SignInResult? result = await _auth.LoginAsync(model);

            if (result?.Succeeded == true)
            {
                _logger.LogInformation("User {Email} signed in.", model.Email);
                return RedirectToAction(nameof(DashboardController.Index), ControllerNames.Dashboard);
            }

            if (result?.IsLockedOut == true)
            {
                _logger.LogWarning("Sign-in blocked: account {Email} is locked out.", model.Email);
                ModelState.AddModelError(string.Empty, "This account is temporarily locked due to multiple failed sign-in attempts. Please try again later.");
                return View(model);
            }

            _logger.LogWarning("Failed sign-in attempt for {Email}.", model.Email);
            // Generic message on purpose: don't reveal whether the email or the password was wrong.
            ModelState.AddModelError(nameof(model.Password), Message.InvalidCredentials);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction(nameof(Login));
            }

            if (!await _auth.SetPasswordUserExistsAsync(userId))
            {
                return RedirectToAction(nameof(Login));
            }

            return View(new SetPasswordViewModel { UserId = userId, Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityResult? result = await _auth.SetPasswordAsync(model);

            if (result is null)
            {
                _logger.LogWarning("Set-password attempt with an invalid/expired link for user {UserId}.", model.UserId);
                ModelState.AddModelError(string.Empty, "This link is invalid or has expired.");
                return View(model);
            }

            if (result.Succeeded)
            {
                _logger.LogInformation("Password set for user {UserId}.", model.UserId);
                TempData["Success"] = "Your password has been set. Please sign in.";
                return RedirectToAction(nameof(Login));
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _auth.SignOutAsync();
            _logger.LogInformation("User signed out.");
            return RedirectToAction(nameof(DashboardController.Index), ControllerNames.Auth);
        }
    }
}
