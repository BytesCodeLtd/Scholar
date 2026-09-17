using Scholar.Common.Paging;
using Scholar.Enums;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IAttendanceService
    {
        bool IsSuperAdmin { get; }

        Task<AttendanceRosterViewModel> BuildRosterAsync(
            DateTime? date, int? instituteId, int? gradeId, string? section, PageParameters tableParams);

        Task<bool> MarkAsync(int studentId, AttendanceStatus status, DateTime date, int? instituteId);

        Task<int?> MarkAllAsync(AttendanceStatus status, DateTime date, int? gradeId, string? section, int? instituteId);
    }
}
