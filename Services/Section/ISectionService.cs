using Scholar.Common.Paging;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface ISectionService
    {
        Task<SectionIndexViewModel> GetIndexAsync(PageParameters tableParams);

        Task<SectionFormViewModel?> GetFormAsync(int? id);

        Task<bool> CreateOrUpdate(int? id, string name, int? instituteId);

        Task<bool> DeleteAsync(int id);
    }
}
