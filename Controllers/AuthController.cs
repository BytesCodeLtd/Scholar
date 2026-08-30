using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;

namespace Scholar.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            bool IsSignedIn = _signInManager.IsSignedIn(User);

            if (IsSignedIn)
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

            ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);

            if (user is not null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(DashboardController.Index), ControllerNames.Dashboard);
                }
            }

            ModelState.AddModelError(nameof(model.Password), Message.InvalidCredentials);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
            }

            return RedirectToAction(nameof(DashboardController.Index), ControllerNames.Auth);
        }
    }
}
