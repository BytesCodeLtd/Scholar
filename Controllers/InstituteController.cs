using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Common.Storage;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class InstituteController : Controller
    {
        private static readonly HashSet<string> AllowedLogoContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/png", "image/jpeg", "image/webp", "image/svg+xml"
        };

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<TestSettings> _settings;
        private readonly IRepository<Institute> _institutes;
        private readonly IFileStorage _fileStorage;
        private readonly IMapper _mapper;

        public InstituteController(
            UserManager<ApplicationUser> userManager,
            IRepository<TestSettings> settings,
            IRepository<Institute> institutes,
            IFileStorage fileStorage,
            IMapper mapper)
        {
            _userManager = userManager;
            _settings = settings;
            _institutes = institutes;
            _fileStorage = fileStorage;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = Roles.SuperAdmin)]
        public IActionResult Index()
        {
            return View(new CreateUserViewModel());
        }

        [HttpGet]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> List([FromQuery] PageParameters tableParams = null)
        {
            tableParams ??= new PageParameters();

            IQueryable<Institute> query = _institutes.Query().AsNoTracking();

            PagedResult<Institute> paged = await _institutes.GetPaginatedByQueryAsync(query, tableParams);

            return View(paged);
        }

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CreateUserViewModel model)
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

            string instituteName = model.InstituteName.Trim();

            Institute? institute = await _institutes.Query().FirstOrDefaultAsync(i => i.Name == instituteName);

            bool isNewInstitute = institute is null;
            institute ??= new Institute { Name = instituteName };

            if (model.Logo is { Length: > 0 })
            {
                institute.LogoUrl = await _fileStorage.UploadAsync(model.Logo, "logos");
            }

            if (isNewInstitute)
            {
                await _institutes.AddAsync(institute);
            }
            else
            {
                _institutes.Update(institute);
            }

            await _institutes.SaveChangesAsync();

            ApplicationUser user = new()
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FullName = model.FullName,
                InstituteId = institute.Id
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            TempData["Success"] = $"User \"{model.Email}\" created as {model.Role}.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Validate Institute Logo: check content type and size.
        /// </summary>
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

        [HttpGet]
        public async Task<IActionResult> PaperSettings(int? instituteId)
        {
            bool isSuperAdmin = User.IsInRole(Roles.SuperAdmin);

            int? targetId = await ResolveInstituteIdAsync(User.GetInstituteId(), isSuperAdmin, instituteId);

            if (targetId is null)
            {
                TempData["Error"] = "No institute is available to configure.";
                return View(BuildEmptyViewModel(isSuperAdmin));
            }

            Institute institute = (await _institutes.GetByIdAsync(targetId.Value))!;

            TestSettings settings = await _settings.Query()
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(p => p.InstituteId == targetId.Value)
                                                    ?? new TestSettings { InstituteId = targetId.Value };

            TestSettingsViewModel vm = ToViewModel(settings, institute);

            await PopulateSwitcherAsync(vm, isSuperAdmin);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaperSettings(TestSettingsViewModel model)
        {
            bool isSuperAdmin = User.IsInRole(Roles.SuperAdmin);

            if (!isSuperAdmin)
            {
                int? userInstituteId = User.GetInstituteId();
                if (userInstituteId is null || userInstituteId != model.InstituteId)
                {
                    return Forbid();
                }
            }

            Institute? institute = await _institutes.GetByIdAsync(model.InstituteId);

            if (institute is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.InstituteName = institute.Name;
                model.LogoUrl = institute.LogoUrl;
                await PopulateSwitcherAsync(model, isSuperAdmin);
                return View(model);
            }

            TestSettings? settings = await _settings.Query()
                                                     .FirstOrDefaultAsync(p => p.InstituteId == model.InstituteId);

            bool isNew = settings is null;
            settings ??= new TestSettings { InstituteId = model.InstituteId };

            _mapper.Map(model, settings);

            if (isNew)
            {
                await _settings.AddAsync(settings);
            }
            else
            {
                _settings.Update(settings);
            }

            // Address lives on the institute itself (shown in the paper header).
            institute.Address = string.IsNullOrWhiteSpace(model.InstituteAddress) ? null : model.InstituteAddress.Trim();
            _institutes.Update(institute);

            await _settings.SaveChangesAsync();

            TempData["Success"] = $"Paper settings saved for \"{institute.Name}\".";

            return RedirectToAction(nameof(PaperSettings), new { instituteId = model.InstituteId });
        }

        private async Task<int?> ResolveInstituteIdAsync(int? userInstituteId, bool isSuperAdmin, int? requestedId)
        {
            if (!isSuperAdmin)
            {
                return userInstituteId;
            }

            bool isExist = await _institutes.Query().AnyAsync(i => i.Id == requestedId);

            if (requestedId is not null && isExist)
            {
                return requestedId;
            }

            return await _institutes.Query()
                                    .OrderBy(i => i.Name)
                                    .Select(i => (int?)i.Id)
                                    .FirstOrDefaultAsync();
        }

        private async Task PopulateSwitcherAsync(TestSettingsViewModel vm, bool isSuperAdmin)
        {
            vm.CanSwitchInstitute = isSuperAdmin;

            if (isSuperAdmin)
            {
                vm.Institutes = await _institutes.Query().AsNoTracking().OrderBy(i => i.Name).ToListAsync();
            }
        }

        private TestSettingsViewModel ToViewModel(TestSettings settings, Institute institute)
        {
            TestSettingsViewModel vm = _mapper.Map<TestSettingsViewModel>(settings);
            vm.InstituteId = institute.Id;
            vm.InstituteName = institute.Name;
            vm.LogoUrl = institute.LogoUrl;
            vm.InstituteAddress = institute.Address;
            return vm;
        }

        private static TestSettingsViewModel BuildEmptyViewModel(bool isSuperAdmin) => new()
        {
            CanSwitchInstitute = isSuperAdmin,
            Institutes = []
        };
    }
}
