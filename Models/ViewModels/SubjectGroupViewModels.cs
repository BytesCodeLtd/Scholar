using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Common.Paging;

namespace Scholar.Models.ViewModels
{
    public class SubjectGroupRow
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public List<string> Sections { get; set; } = [];

        public List<string> Subjects { get; set; } = [];

        public string? Description { get; set; }

        public string? InstituteName { get; set; }

        public bool IsActive { get; set; }
    }

    public class SubjectGroupOption
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int InstituteId { get; set; }

        public List<int> SectionIds { get; set; } = [];
    }

    public class SubjectGroupIndexViewModel
    {
        public PagedResult<SubjectGroupRow> Groups { get; set; } = new();

        public bool IsSuperAdmin { get; set; }

        public IReadOnlyList<SelectListItem> InstituteOptions { get; set; } = [];

        public IReadOnlyList<SubjectGroupOption> ClassOptions { get; set; } = [];

        public IReadOnlyList<SubjectGroupOption> SectionOptions { get; set; } = [];

        public IReadOnlyList<SubjectGroupOption> SubjectOptions { get; set; } = [];
    }

    public class SubjectGroupFormViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? ClassId { get; set; }

        public int? InstituteId { get; set; }

        public bool IsSuperAdmin { get; set; }

        public IReadOnlyList<SelectListItem> InstituteOptions { get; set; } = [];

        public IReadOnlyList<SubjectGroupOption> ClassOptions { get; set; } = [];

        public IReadOnlyList<SubjectGroupOption> SectionOptions { get; set; } = [];

        public IReadOnlyList<SubjectGroupOption> SubjectOptions { get; set; } = [];

        public HashSet<int> SelectedSectionIds { get; set; } = [];

        public HashSet<int> SelectedSubjectIds { get; set; } = [];
    }
}
