using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Common.Paging;

namespace Scholar.Models.ViewModels
{
    /// <summary>A single row in the classes DataTable.</summary>
    public class ClassRow
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>The section this class belongs to.</summary>
        public string SectionName { get; set; } = string.Empty;

        /// <summary>Owning institute's name — shown to SuperAdmins, who see every institute's classes.</summary>
        public string? InstituteName { get; set; }

        public bool IsActive { get; set; }
    }

    /// <summary>Backs the Class management page (add form + classes DataTable).</summary>
    public class ClassIndexViewModel
    {
        public PagedResult<ClassRow> Classes { get; set; } = new();

        public bool IsSuperAdmin { get; set; }

        /// <summary>Sections the class can be placed under (own institute's, or all for SuperAdmin).</summary>
        public IReadOnlyList<SelectListItem> SectionOptions { get; set; } = [];
    }

    /// <summary>Backs the Class create/update form. Id == 0 means create.</summary>
    public class ClassFormViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int? SectionId { get; set; }

        /// <summary>Sections the class can be placed under.</summary>
        public IReadOnlyList<SelectListItem> SectionOptions { get; set; } = [];
    }
}
