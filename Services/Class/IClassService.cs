using Scholar.Common.Paging;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IClassService
    {
        Task<ClassIndexViewModel> GetIndexAsync(PageParameters tableParams);

        /// <summary>Builds the create/update form. Pass null to create, an id to edit
        /// (returns null when the id doesn't resolve to a class in the caller's tenant).</summary>
        Task<ClassFormViewModel?> GetFormAsync(int? id);

        /// <summary>Creates (id null) or updates (id set) a class under the given section.
        /// Returns false on invalid input or an unreachable section.</summary>
        Task<bool> CreateOrUpdate(int? id, string name, int? sectionId);
    }
}
