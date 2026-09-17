using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Common.Paging;

namespace Scholar.Models.ViewModels
{
    public class ClassRow
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SectionName { get; set; } = string.Empty;

        public string? InstituteName { get; set; }

        public bool IsActive { get; set; }
    }

    public class ClassIndexViewModel
    {
        public PagedResult<ClassRow> Classes { get; set; } = new();

        public bool IsSuperAdmin { get; set; }

        public IReadOnlyList<SelectListItem> SectionOptions { get; set; } = [];
    }

    public class ClassFormViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int? SectionId { get; set; }

        public IReadOnlyList<SelectListItem> SectionOptions { get; set; } = [];
    }
}
