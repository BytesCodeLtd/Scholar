using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Enums;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class AttendanceController(IAttendanceService attendance, ILogger<AttendanceController> logger) : Controller
    {
        private readonly IAttendanceService _attendance = attendance;
        private readonly ILogger<AttendanceController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date, int? instituteId, int? gradeId, string? section,
            [FromQuery] PageParameters? tableParams = null)
        {
            AttendanceRosterViewModel model = await _attendance.BuildRosterAsync(
                date, instituteId, gradeId, section, tableParams ?? new PageParameters());

            return View(model);
        }

        /// <summary>
        /// Sets a single student's status for a day (invoked from the roster row
        /// menu), then returns to the same roster view.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mark(
            int studentId, AttendanceStatus status, DateTime date,
            int? gradeId, string? section, int? instituteId, int page = 1)
        {
            if (!await _attendance.MarkAsync(studentId, status, date, instituteId))
            {
                _logger.LogWarning("Mark attendance failed for student {StudentId}.", studentId);
                TempData["Error"] = Message.AccountNotLinkedToInstitute;
                return RedirectToAction(nameof(Index));
            }

            DateTime day = date.Date;
            _logger.LogInformation("Student {StudentId} marked {Status} for {Day:yyyy-MM-dd}.", studentId, status, day);
            TempData["Success"] = MsgKey.Attendance.Marked(status, day);
            return RedirectToAction(nameof(Index), new
            {
                date = day.ToString("yyyy-MM-dd"),
                instituteId = _attendance.IsSuperAdmin ? instituteId : null,
                gradeId,
                section,
                page
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAll(AttendanceStatus status, DateTime date, int? gradeId, string? section, int? instituteId, int page = 1)
        {
            int? count = await _attendance.MarkAllAsync(status, date, gradeId, section, instituteId);

            if (count is null)
            {
                _logger.LogWarning("Mark-all attendance failed for grade {GradeId}.", gradeId);
                TempData["Error"] = gradeId is null ? Message.SelectClassToMark : Message.AccountNotLinkedToInstitute;
                return RedirectToAction(nameof(Index));
            }

            DateTime day = date.Date;
            _logger.LogInformation("Marked {Count} student(s) {Status} for {Day:yyyy-MM-dd}.", count.Value, status, day);
            TempData["Success"] = MsgKey.Attendance.MarkedAll(count.Value, status, day);
            return RedirectToAction(nameof(Index), new
            {
                date = day.ToString("yyyy-MM-dd"),
                instituteId = _attendance.IsSuperAdmin ? instituteId : null,
                gradeId,
                section,
                page
            });
        }
    }
}
