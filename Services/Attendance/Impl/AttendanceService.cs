using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class AttendanceService(
        IRepository<Attendance> attendance,
        IRepository<Student> students,
        IRepository<InstituteClass> classes,
        IRepository<Institute> institutes,
        ITenantProvider tenant) : IAttendanceService
    {
        private readonly IRepository<Attendance> _attendance = attendance;
        private readonly IRepository<Student> _students = students;
        private readonly IRepository<InstituteClass> _classes = classes;
        private readonly IRepository<Institute> _institutes = institutes;
        private readonly ITenantProvider _tenant = tenant;

        public bool IsSuperAdmin => _tenant.IsSuperAdmin;

        public async Task<AttendanceRosterViewModel> BuildRosterAsync(DateTime? date, int? instituteId, int? gradeId, string? section, PageParameters tableParams)
        {
            int? targetInstituteId = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

            AttendanceRosterViewModel model = new()
            {
                Date = (date ?? DateTime.Today).Date,
                InstituteId = targetInstituteId,
                GradeId = gradeId,
                Section = string.IsNullOrWhiteSpace(section) ? null : section.Trim(),
                ShowInstitute = _tenant.IsSuperAdmin
            };

            await PopulateOptionsAsync(model);

            // Only build the roster once we know both the institute and the class.
            if (targetInstituteId is not null && gradeId is not null)
            {
                IQueryable<Student> students = _students.Query().Where(s => s.IsActive && s.InstituteId == targetInstituteId && s.ClassId == gradeId);

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

                Common.Paging.PagedResult<AttendanceRosterRow> paged = await _students.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);

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

            return model;
        }

        public async Task<bool> MarkAsync(int studentId, AttendanceStatus status, DateTime date, int? instituteId)
        {
            int? targetInstituteId = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

            DateTime day = date.Date;

            if (targetInstituteId is null)
            {
                return false;
            }

            // The student must belong to this institute.
            bool ownsStudent = await _students.Query().AnyAsync(s => s.Id == studentId && s.InstituteId == targetInstituteId);

            if (!ownsStudent)
            {
                return false;
            }

            // Upsert keyed on the unique index (StudentId + Date).
            Attendance? row = await _attendance.Query().FirstOrDefaultAsync(a => a.StudentId == studentId && a.Date == day);

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
                row.Status = status;
            }

            await _attendance.SaveChangesAsync();

            return true;
        }

        public async Task<int?> MarkAllAsync(AttendanceStatus status, DateTime date, int? gradeId, string? section, int? instituteId)
        {
            int? targetInstituteId = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

            DateTime day = date.Date;

            if (targetInstituteId is null || gradeId is null)
            {
                return null;
            }

            string? sec = string.IsNullOrWhiteSpace(section) ? null : section.Trim();

            IQueryable<Student> studentsQuery = _students.Query().Where(s => s.IsActive && s.InstituteId == targetInstituteId && s.ClassId == gradeId);

            if (sec is not null)
            {
                studentsQuery = studentsQuery.Where(s => s.Section == sec);
            }

            List<int> studentIds = await studentsQuery.Select(s => s.Id).ToListAsync();

            // The day's existing marks for these students, so we can update in place.
            Dictionary<int, Attendance> existing = await _attendance.Query()
                .Where(a => a.Date == day && studentIds.Contains(a.StudentId))
                .ToDictionaryAsync(a => a.StudentId);

            foreach (int studentId in studentIds)
            {
                if (existing.TryGetValue(studentId, out Attendance? row))
                {
                    row.Status = status;
                }
                else
                {
                    await _attendance.AddAsync(new Attendance
                    {
                        InstituteId = targetInstituteId.Value,
                        StudentId = studentId,
                        Date = day,
                        Status = status
                    });
                }
            }

            await _attendance.SaveChangesAsync();

            return studentIds.Count;
        }

        private async Task PopulateOptionsAsync(AttendanceRosterViewModel model)
        {
            model.GradeOptions = await _classes.Query()
                                               .Where(c => c.IsActive)
                                               .OrderBy(c => c.Name)
                                               .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                                               .ToListAsync();

            if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await _institutes.Query()
                                                           .Where(i => i.IsActive)
                                                           .OrderBy(i => i.Name)
                                                           .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                                                           .ToListAsync();
            }
        }
    }
}
