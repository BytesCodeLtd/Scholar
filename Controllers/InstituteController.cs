using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class InstituteController : Controller
    {
        private static readonly HashSet<string> AllowedLogoContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/png", "image/jpeg", "image/webp", "image/svg+xml"
        };

        private readonly IInstituteService _institute;
        private readonly ILogger<InstituteController> _logger;

        public InstituteController(IInstituteService institute, ILogger<InstituteController> logger)
        {
            _institute = institute;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> CreateOrUpdate(int? id)
        {
            CreateUserViewModel? model = await _institute.BuildInstituteFormAsync(id);

            if (model is null)
            {
                TempData["Error"] = "Institute not found.";
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> List([FromQuery] PageParameters? tableParams = null)
        {
            PagedResult<Institute> paged = await _institute.GetInstitutesAsync(tableParams ?? new PageParameters());
            return View(paged);
        }

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(CreateUserViewModel model)
        {
            if (!Roles.All.Contains(model.Role))
            {
                ModelState.AddModelError(nameof(model.Role), MsgKey.Validation.Invalid(Key.Role));
            }

            ValidateLogo(model.Logo);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!await _institute.SaveInstituteAsync(model))
            {
                _logger.LogWarning("Save institute failed for \"{Name}\" ({Email}).", model.InstituteName, model.Email);
                ModelState.AddModelError(string.Empty, "Couldn't save. The institute name or email may already be in use, or the details are invalid.");
                return View(model);
            }

            _logger.LogInformation("Institute \"{Name}\" {Action}.", model.InstituteName, model.InstituteId is null ? "created" : "updated");
            TempData["Success"] = model.InstituteId is null ? "Institute created." : "Institute updated.";
            return RedirectToAction(nameof(List));
        }

        // Validate the institute logo (type + size) straight onto ModelState.
        private void ValidateLogo(IFormFile? logo)
        {
            if (logo is null || logo.Length == 0)
            {
                return;
            }

            const long maxSizeBytes = 2 * 1024 * 1024; // 2 MB

            if (!AllowedLogoContentTypes.Contains(logo.ContentType))
            {
                ModelState.AddModelError(nameof(CreateUserViewModel.Logo), Message.LogoInvalidType);
            }

            if (logo.Length > maxSizeBytes)
            {
                ModelState.AddModelError(nameof(CreateUserViewModel.Logo), Message.LogoTooLarge);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _institute.DeactivateAsync(id);
            if (deleted)
            {
                _logger.LogInformation("Institute {Id} deactivated.", id);
            }
            else
            {
                _logger.LogWarning("Deactivate failed: institute {Id} not found.", id);
            }

            TempData[deleted ? "Success" : "Error"] = deleted ? "Institute deactivated." : "Institute not found.";
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            bool activated = await _institute.ActivateAsync(id);
            if (activated)
            {
                _logger.LogInformation("Institute {Id} activated.", id);
            }
            else
            {
                _logger.LogWarning("Activate failed: institute {Id} not found.", id);
            }

            TempData[activated ? "Success" : "Error"] = activated ? "Institute activated." : "Institute not found.";
            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public async Task<IActionResult> PaperSettings(int? instituteId)
        {
            TestSettingsViewModel? vm = await _institute.BuildPaperSettingsAsync(instituteId);

            if (vm is null)
            {
                TempData["Error"] = "No institute is available to configure.";
                vm = new TestSettingsViewModel { CanSwitchInstitute = _institute.IsSuperAdmin };
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaperSettings(TestSettingsViewModel model)
        {
            if (!_institute.CanEditPaperSettings(model.InstituteId))
            {
                _logger.LogWarning("Blocked paper-settings edit for institute {Id} (not permitted).", model.InstituteId);
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await _institute.PopulatePaperSettingsChromeAsync(model);
                return View(model);
            }

            string? instituteName = await _institute.SavePaperSettingsAsync(model);

            if (instituteName is null)
            {
                _logger.LogWarning("Save paper settings failed: institute {Id} not found.", model.InstituteId);
                return NotFound();
            }

            _logger.LogInformation("Paper settings saved for institute {Id}.", model.InstituteId);
            TempData["Success"] = $"Paper settings saved for \"{instituteName}\".";
            return RedirectToAction(nameof(PaperSettings), new { instituteId = model.InstituteId });
        }
    }
}
