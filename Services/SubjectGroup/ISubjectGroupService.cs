using Scholar.Common.Paging;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface ISubjectGroupService
    {
        Task<SubjectGroupIndexViewModel> GetIndexAsync(PageParameters tableParams);

        Task<SubjectGroupFormViewModel?> GetFormAsync(int? id);

        Task<bool> CreateOrUpdate(int? id, string name, string? description, int? classId, int[] sectionIds, int[] subjectIds, int? instituteId);

        Task<bool> DeleteAsync(int id);
    }
}
