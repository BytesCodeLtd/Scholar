using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class AttendanceController : Controller
    {
        private readonly IRepository<Attendance> _attendance;
        private readonly IRepository<Student> _students;
        private readonly IRepository<Grade> _grades;
        private readonly IRepository<Institute> _institutes;

        public AttendanceController(
            IRepository<Attendance> attendance,
            IRepository<Student> students,
            IRepository<Grade> grades,
            IRepository<Institute> institutes)
        {
            _attendance = attendance;
            _students = students;
            _grades = grades;
            _institutes = institutes;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date, int? instituteId, int? gradeId, string? section,
            [FromQuery] PageParameters? tableParams = null)
        {
            tableParams ??= new PageParameters();

            UserScope scope = GetScope();

            int? targetInstituteId = scope.IsSuperAdmin ? instituteId : scope.InstituteId;

            AttendanceRosterViewModel model = new()
            {
                Date = (date ?? DateTime.Today).Date,
                InstituteId = targetInstituteId,
                GradeId = gradeId,
                Section = string.IsNullOrWhiteSpace(section) ? null : section.Trim(),
                ShowInstitute = scope.IsSuperAdmin
            };

            await PopulateOptionsAsync(model, scope.IsSuperAdmin);

            // Only build the roster once we know both the institute and the class.
            if (targetInstituteId is not null && gradeId is not null)
            {
                IQueryable<Student> students = _students.Query()
                    .Where(s => s.IsActive && s.InstituteId == targetInstituteId && s.GradeId == gradeId);

                if (model.Section is not null)
                {
                    students = students.Where(s => s.Section == model.Section);
                }

                IQueryable<AttendanceRosterRow> rows = students.Select(s => new AttendanceRosterRow
                {
                    StudentId = s.Id,
                    Name = s.FullName,
                    RollNumber = s.RollNumber,
                    Section = s.Section
                });

                // There's no Id column on the row, so the default/unset "Id" sort
                // (and a StudentId sort) falls back to Name.
                string orderBy = tableParams.OrderBy;
                if (string.IsNullOrWhiteSpace(orderBy)
                    || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase)
                    || orderBy.StartsWith("StudentId", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = "Name asc";
                }
                rows = rows.OrderBy(orderBy);

                Scholar.Common.Paging.PagedResult<AttendanceRosterRow> paged =
                    await _students.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);

                // Overlay any marks already recorded for this day onto the page's rows.
                List<int> pageIds = paged.Items.Select(r => r.StudentId).ToList();
                Dictionary<int, AttendanceStatus> existing = await _attendance.Query()
                    .Where(a => a.Date == model.Date && pageIds.Contains(a.StudentId))
                    .ToDictionaryAsync(a => a.StudentId, a => a.Status);

                foreach (AttendanceRosterRow row in paged.Items)
                {
                    if (existing.TryGetValue(row.StudentId, out AttendanceStatus status))
                    {
                        row.Status = status;
                    }
                }

                model.Roster = paged;
                model.Loaded = true;
            }

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
            UserScope scope = GetScope();

            int? targetInstituteId = scope.IsSuperAdmin ? instituteId : scope.InstituteId;

            if (targetInstituteId is null)
            {
                TempData["Error"] = "Your account is not linked to an institute.";
                return RedirectToAction(nameof(Index));
            }

            // The student must belong to this institute.
            bool ownsStudent = await _students.Query()
                .AnyAsync(s => s.Id == studentId && s.InstituteId == targetInstituteId);

            if (!ownsStudent)
            {
                return Forbid();
            }

            DateTime day = date.Date;

            // Upsert keyed on the unique index (StudentId + Date).
            Attendance? row = await _attendance.Query()
                .FirstOrDefaultAsync(a => a.StudentId == studentId && a.Date == day);

            if (row is null)
            {
                await _attendance.AddAsync(new Attendance
                {
                    InstituteId = targetInstituteId.Value,
                    StudentId = studentId,
                    Date = day,
                    Status = status
                });
            }
            else
            {
                row.Status = status; // tracked change; UpdatedAt stamped on save
            }

            await _attendance.SaveChangesAsync();

            TempData["Success"] = $"Marked {status} on {day:d MMM yyyy}.";

            return RedirectToAction(nameof(Index), new
            {
                date = day.ToString("yyyy-MM-dd"),
                instituteId = scope.IsSuperAdmin ? targetInstituteId : null,
                gradeId,
                section,
                page
            });
        }

        private async Task PopulateOptionsAsync(AttendanceRosterViewModel model, bool isSuperAdmin)
        {
            model.GradeOptions = await _grades.Query()
                                              .Where(g => g.IsActive)
                                              .OrderBy(g => g.Name)
                                              .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name })
                                              .ToListAsync();

            if (isSuperAdmin)
            {
                model.InstituteOptions = await _institutes.Query()
                                                          .Where(i => i.IsActive)
                                                          .OrderBy(i => i.Name)
                                                          .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                                                          .ToListAsync();
            }
        }
        private UserScope GetScope()
            => new() { IsSuperAdmin = User.IsInRole(Roles.SuperAdmin), InstituteId = User.GetInstituteId() };
    }
}
