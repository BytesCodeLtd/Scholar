using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Constants;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class AccountController(IAccountService account, ILogger<AccountController> logger) : Controller
    {
        private readonly IAccountService _account = account;
        private readonly ILogger<AccountController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AccountViewModel? model = await _account.GetProfileAsync();

            if (model is null)
            {
                return RedirectToAction(nameof(AuthController.Login), ControllerNames.Auth);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool updated = await _account.UpdateProfileAsync(model);

            if (!updated)
            {
                _logger.LogWarning("Account update failed for {Email}.", model.Email);
                ModelState.AddModelError(nameof(model.Email), "Couldn't update your account. That email may already be in use.");
                return View(model);
            }

            _logger.LogInformation("Account details updated for {Email}.", model.Email);
            TempData["Success"] = "Account details updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
