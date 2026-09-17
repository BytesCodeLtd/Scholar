using Scholar.Common.Paging;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IClassService
    {
        Task<ClassIndexViewModel> GetIndexAsync(PageParameters tableParams);

        Task<ClassFormViewModel?> GetFormAsync(int? id);

        Task<bool> CreateOrUpdate(int? id, string name, int? sectionId);

        Task<bool> DeleteAsync(int id);
    }
}
