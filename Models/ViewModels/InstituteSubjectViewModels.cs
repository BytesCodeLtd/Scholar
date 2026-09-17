using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Common.Paging;
using Scholar.Enums;

namespace Scholar.Models.ViewModels
{
    public class InstituteSubjectRow
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public SubjectType Type { get; set; }

        public string? InstituteName { get; set; }

        public bool IsActive { get; set; }
    }

    public class InstituteSubjectIndexViewModel
    {
        public PagedResult<InstituteSubjectRow> Subjects { get; set; } = new();

        public bool IsSuperAdmin { get; set; }

        public IReadOnlyList<SelectListItem> InstituteOptions { get; set; } = [];
    }

    public class InstituteSubjectFormViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public SubjectType Type { get; set; }

        public bool IsSuperAdmin { get; set; }

        public IReadOnlyList<SelectListItem> InstituteOptions { get; set; } = [];
    }
}
