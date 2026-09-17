using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Email;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Common.Storage;
using Scholar.Constants;
using Scholar.Data;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class InstituteService(
        UserManager<ApplicationUser> userManager,
        IRepository<TestSettings> settings,
        IRepository<Institute> institutes,
        IFileStorage fileStorage,
        IEmailSender emailSender,
        IEmailTemplateRenderer templates,
        ScholarDbContext db,
        ITenantProvider tenant,
        LinkGenerator linkGenerator,
        IHttpContextAccessor http,
        ILogger<InstituteService> logger,
        IMapper mapper) : IInstituteService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IRepository<TestSettings> _settings = settings;
        private readonly IRepository<Institute> _institutes = institutes;
        private readonly IFileStorage _fileStorage = fileStorage;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IEmailTemplateRenderer _templates = templates;
        private readonly ScholarDbContext _db = db;
        private readonly ITenantProvider _tenant = tenant;
        private readonly LinkGenerator _linkGenerator = linkGenerator;
        private readonly IHttpContextAccessor _http = http;
        private readonly ILogger<InstituteService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<CreateUserViewModel?> BuildInstituteFormAsync(int? id)
        {
            if (id is null)
            {
                return new CreateUserViewModel();
            }

            Institute? institute = await _institutes.Query()
                                                    .AsNoTracking()
                                                    .Include(i => i.Users)
                                                    .FirstOrDefaultAsync(i => i.Id == id.Value);

            if (institute is null)
            {
                return null;
            }

            CreateUserViewModel model = new()
            {
                InstituteId = institute.Id,
                InstituteName = institute.Name,
                InstituteAddress = institute.Address,
                LogoUrl = institute.LogoUrl
            };

            ApplicationUser? admin = institute.Users.FirstOrDefault();

            if (admin is not null)
            {
                model.FullName = admin.FullName ?? string.Empty;
                model.Email = admin.Email ?? string.Empty;
                model.Role = (await _userManager.GetRolesAsync(admin)).FirstOrDefault() ?? string.Empty;
            }

            return model;
        }

        public bool IsSuperAdmin => _tenant.IsSuperAdmin;

        public Task<PagedResult<Institute>> GetInstitutesAsync(PageParameters tableParams)
            => _institutes.GetPaginatedByQueryAsync(_institutes.Query().AsNoTracking(), tableParams);

        // Role and logo are validated in the controller; here we do the DB work.
        public Task<bool> SaveInstituteAsync(CreateUserViewModel model)
            => model.InstituteId is not null ? UpdateAsync(model) : CreateAsync(model);

        private async Task<bool> CreateAsync(CreateUserViewModel model)
        {
            string instituteName = model.InstituteName.Trim();

            if (await _institutes.Query().AnyAsync(i => i.Name == instituteName))
            {
                return false;
            }

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return false;
            }

            Institute institute = new()
            {
                Name = instituteName,
                Address = string.IsNullOrWhiteSpace(model.InstituteAddress) ? null : model.InstituteAddress.Trim()
            };

            if (model.Logo is { Length: > 0 })
            {
                institute.LogoUrl = await _fileStorage.UploadAsync(model.Logo, "logos");
            }

            ApplicationUser user = new()
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FullName = model.FullName
            };

            bool invite = string.IsNullOrWhiteSpace(model.Password);

            IdentityResult? failure = null;

            var strategy = _db.Database.CreateExecutionStrategy();

            bool created = await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();

                await _institutes.AddAsync(institute);
                await _institutes.SaveChangesAsync();

                user.InstituteId = institute.Id;

                IdentityResult result = invite
                    ? await _userManager.CreateAsync(user)
                    : await _userManager.CreateAsync(user, model.Password!);

                if (!result.Succeeded)
                {
                    failure = result;
                    await transaction.RollbackAsync();
                    return false;
                }

                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!roleResult.Succeeded)
                {
                    failure = roleResult;
                    await transaction.RollbackAsync();
                    return false;
                }

                await transaction.CommitAsync();
                return true;
            });

            if (!created)
            {
                return false;
            }

            if (invite)
            {
                await SendSetPasswordEmailAsync(user);
            }

            return true;
        }

        private async Task<bool> UpdateAsync(CreateUserViewModel model)
        {
            Institute? institute = await _institutes.Query()
                                                    .Include(i => i.Users)
                                                    .FirstOrDefaultAsync(i => i.Id == model.InstituteId);

            if (institute is null)
            {
                return false;
            }

            string instituteName = model.InstituteName.Trim();

            bool nameTaken = await _institutes.Query()
                                              .AnyAsync(i => i.Name == instituteName && i.Id != institute.Id);
            if (nameTaken)
            {
                return false;
            }

            institute.Name = instituteName;
            institute.Address = string.IsNullOrWhiteSpace(model.InstituteAddress) ? null : model.InstituteAddress.Trim();

            if (model.Logo is { Length: > 0 })
            {
                institute.LogoUrl = await _fileStorage.UploadAsync(model.Logo, "logos");
            }

            _institutes.Update(institute);
            await _institutes.SaveChangesAsync();

            ApplicationUser? admin = institute.Users.FirstOrDefault();

            if (admin is not null)
            {
                admin.FullName = model.FullName;
                admin.Email = model.Email;
                admin.UserName = model.Email;

                IdentityResult update = await _userManager.UpdateAsync(admin);
                if (!update.Succeeded)
                {
                    return false;
                }

                IList<string> currentRoles = await _userManager.GetRolesAsync(admin);
                if (!currentRoles.Contains(model.Role))
                {
                    await _userManager.RemoveFromRolesAsync(admin, currentRoles);
                    await _userManager.AddToRoleAsync(admin, model.Role);
                }

                // Only touch the password if a new one was entered.
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    string token = await _userManager.GeneratePasswordResetTokenAsync(admin);
                    IdentityResult reset = await _userManager.ResetPasswordAsync(admin, token, model.Password);
                    if (!reset.Succeeded)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Task<bool> DeactivateAsync(int id) => _institutes.SoftDeleteAsync(id);

        public Task<bool> ActivateAsync(int id) => _institutes.ActivateAsync(id);

        public bool CanEditPaperSettings(int instituteId)
            => _tenant.IsSuperAdmin || _tenant.InstituteId == instituteId;

        public async Task<TestSettingsViewModel?> BuildPaperSettingsAsync(int? instituteId)
        {
            int? targetId = await ResolveInstituteIdAsync(instituteId);

            if (targetId is null)
            {
                return null;
            }

            Institute institute = (await _institutes.GetByIdAsync(targetId.Value))!;

            TestSettings settings = await _settings.Query()
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(p => p.InstituteId == targetId.Value)
                                                    ?? new TestSettings { InstituteId = targetId.Value };

            TestSettingsViewModel vm = ToViewModel(settings, institute);
            await PopulateSwitcherAsync(vm);

            return vm;
        }

        public async Task PopulatePaperSettingsChromeAsync(TestSettingsViewModel model)
        {
            Institute? institute = await _institutes.GetByIdAsync(model.InstituteId);
            if (institute is not null)
            {
                model.InstituteName = institute.Name;
                model.LogoUrl = institute.LogoUrl;
            }

            await PopulateSwitcherAsync(model);
        }

        public async Task<string?> SavePaperSettingsAsync(TestSettingsViewModel model)
        {
            Institute? institute = await _institutes.GetByIdAsync(model.InstituteId);

            if (institute is null)
            {
                return null;
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

            return institute.Name;
        }

        private async Task<bool> SendSetPasswordEmailAsync(ApplicationUser user)
        {
            try
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                string? link = _linkGenerator.GetUriByAction(
                    _http.HttpContext!,
                    action: "SetPassword",
                    controller: "Auth",
                    values: new { userId = user.Id, token = code });

                string html = await _templates.RenderAsync("SetPassword.html", new Dictionary<string, string>
                {
                    ["FullName"] = System.Net.WebUtility.HtmlEncode(user.FullName),
                    ["Link"] = link ?? string.Empty
                });

                await _emailSender.SendAsync(user.Email!, "Set up your Scholar account", html);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send set-password email to {Email}", user.Email);
                return false;
            }
        }

        private async Task<int?> ResolveInstituteIdAsync(int? requestedId)
        {
            if (!_tenant.IsSuperAdmin)
            {
                return _tenant.InstituteId;
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

        private async Task PopulateSwitcherAsync(TestSettingsViewModel vm)
        {
            vm.CanSwitchInstitute = _tenant.IsSuperAdmin;

            if (_tenant.IsSuperAdmin)
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
    }
}
