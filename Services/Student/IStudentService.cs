using Scholar.Common.Paging;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IStudentService
    {
        bool CanOpenAdmissionForm { get; }

        Task<PagedResult<StudentRow>> GetRosterAsync(PageParameters tableParams);

        Task<StudentDetailsViewModel?> GetDetailsAsync(int id);

        Task PopulateCreateOptionsAsync(CreateStudentViewModel model);

        Task<bool> CreateAsync(CreateStudentViewModel model);

        Task PopulateRegisterOptionsAsync(RegisterStudentViewModel model);

        Task<bool> RegisterAsync(RegisterStudentViewModel model);

        Task<bool> DeactivateAsync(int id);

        Task<bool> ActivateAsync(int id);
    }
}
