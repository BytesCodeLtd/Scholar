using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;

namespace Scholar.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction(nameof(AuthController.Login), ControllerNames.Auth);
            }

            AccountViewModel model = new()
            {
                FullName = user.FullName,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AccountViewModel model)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction(nameof(AuthController.Login), ControllerNames.Auth);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email!.Trim();

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                ApplicationUser? existing = await _userManager.FindByEmailAsync(email);

                if (existing is not null && existing.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(model.Email), "That email is already in use.");
                    return View(model);
                }

                IdentityResult emailResult = await _userManager.SetEmailAsync(user, email);
                IdentityResult userNameResult = await _userManager.SetUserNameAsync(user, email);

                foreach (IdentityError error in emailResult.Errors.Concat(userNameResult.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                user.EmailConfirmed = true;
            }

            user.FullName = model.FullName!.Trim();

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["Success"] = "Account details updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
