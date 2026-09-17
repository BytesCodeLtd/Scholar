using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Common.Paging;

namespace Scholar.Models.ViewModels
{
    public class SectionRow
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? InstituteName { get; set; }

        public bool IsActive { get; set; }
    }

    public class SectionIndexViewModel
    {
        public PagedResult<SectionRow> Sections { get; set; } = new();

        public bool IsSuperAdmin { get; set; }

        public IReadOnlyList<SelectListItem> InstituteOptions { get; set; } = [];
    }

    /// <summary>Backs the Section create/update form. Id == 0 means create.</summary>
    public class SectionFormViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>True when the current user is a SuperAdmin, who must pick an institute on create.</summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>Institute choices for the SuperAdmin create dropdown.</summary>
        public IReadOnlyList<SelectListItem> InstituteOptions { get; set; } = [];
    }
}
