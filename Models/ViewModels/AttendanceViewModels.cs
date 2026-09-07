using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Common.Paging;
using Scholar.Enums;

namespace Scholar.Models.ViewModels
{
    public class AttendanceRosterViewModel
    {
        public DateTime Date { get; set; } = DateTime.Today;

        public int? InstituteId { get; set; }

        public int? GradeId { get; set; }

        public string? Section { get; set; }

        /// <summary>True once a class has been chosen and the roster is populated.</summary>
        public bool Loaded { get; set; }

        /// <summary>Super admins pick the institute; institute admins are fixed to their own.</summary>
        public bool ShowInstitute { get; set; }

        public IEnumerable<SelectListItem> InstituteOptions { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> GradeOptions { get; set; } = new List<SelectListItem>();

        public PagedResult<AttendanceRosterRow> Roster { get; set; } = new();
    }

    /// <summary>One student's row in the roster, showing any existing mark for the day.</summary>
    public class AttendanceRosterRow
    {
        public int StudentId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? RollNumber { get; set; }

        public string? Section { get; set; }

        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    }

    /// <summary>A single attendance record shown in a student's history.</summary>
    public class AttendanceHistoryItem
    {
        public DateTime Date { get; set; }

        public AttendanceStatus Status { get; set; }
    }
}
