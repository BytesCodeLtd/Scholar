using Scholar.Common.Paging;
using Scholar.Models;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IInstituteService
    {
        bool IsSuperAdmin { get; }

        Task<CreateUserViewModel?> BuildInstituteFormAsync(int? id);

        Task<PagedResult<Institute>> GetInstitutesAsync(PageParameters tableParams);

        Task<bool> SaveInstituteAsync(CreateUserViewModel model);

        Task<bool> DeactivateAsync(int id);

        Task<bool> ActivateAsync(int id);

        bool CanEditPaperSettings(int instituteId);

        Task<TestSettingsViewModel?> BuildPaperSettingsAsync(int? instituteId);

        Task PopulatePaperSettingsChromeAsync(TestSettingsViewModel model);

        Task<string?> SavePaperSettingsAsync(TestSettingsViewModel model);
    }
}
