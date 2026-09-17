using Scholar.Common.Paging;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface ISectionService
    {
        Task<SectionIndexViewModel> GetIndexAsync(PageParameters tableParams);

        /// <summary>Builds the create/update form. Pass null to create, an id to edit
        /// (returns null when the id doesn't resolve to a section in the caller's tenant).</summary>
        Task<SectionFormViewModel?> GetFormAsync(int? id);

        /// <summary>Creates (id null) or updates (id set) a section. Returns false on invalid input.</summary>
        Task<bool> CreateOrUpdate(int? id, string name, int? instituteId);
    }
}
