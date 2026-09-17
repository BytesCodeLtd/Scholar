using Scholar.Common.Paging;
using Scholar.Enums;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IInstituteSubjectService
    {
        Task<InstituteSubjectIndexViewModel> GetIndexAsync(PageParameters tableParams);

        Task<InstituteSubjectFormViewModel?> GetFormAsync(int? id);

        Task<bool> CreateOrUpdate(int? id, string name, string code, SubjectType type, int? instituteId);

        Task<bool> DeleteAsync(int id);
    }
}
