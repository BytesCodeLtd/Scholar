using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class StudentController : Controller
    {
        private readonly IRepository<Student> _students;
        private readonly IRepository<Grade> _grades;
        private readonly IRepository<Institute> _institutes;

        public StudentController(
            IRepository<Student> students,
            IRepository<Grade> grades,
            IRepository<Institute> institutes)
        {
            _students = students;
            _grades = grades;
            _institutes = institutes;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters tableParams = null)
        {
            tableParams ??= new PageParameters();

            (bool isSuperAdmin, int? instituteId) = GetScope();

            IQueryable<Student> students = _students.Query().Where(s => s.IsActive);

            if (!isSuperAdmin)
            {
                students = students.Where(s => s.InstituteId == instituteId);
            }

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                students = students.Where(s =>
                    s.FullName.Contains(term) ||
                    (s.RollNumber != null && s.RollNumber.Contains(term)) ||
                    s.Grade.Name.Contains(term) ||
                    (s.Section != null && s.Section.Contains(term)));
            }

            IQueryable<StudentRow> rows = students.Select(s => new StudentRow
            {
                Id = s.Id,
                Name = s.FullName,
                RollNumber = s.RollNumber,
                Class = s.Grade.Name,
                Section = s.Section,
                Guardian = s.GuardianName,
                Phone = s.PhoneNumber,
                Institute = s.Institute.Name
            });

            // Id isn't a display column, so the default/unset sort falls back to Name.
            string orderBy = tableParams.OrderBy;
            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "Name asc";
            }
            rows = rows.OrderBy(orderBy);

            Scholar.Common.Paging.PagedResult<StudentRow> paged = await _students.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);

            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            (bool isSuperAdmin, int? instituteId) = GetScope();

            if (!isSuperAdmin && instituteId is null)
            {
                TempData["Error"] = "Your account is not linked to an institute.";
                return RedirectToAction(nameof(Index));
            }

            CreateStudentViewModel model = new();
            await PopulateOptionsAsync(model, isSuperAdmin);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel model)
        {
            (bool isSuperAdmin, int? instituteId) = GetScope();

            // A super admin picks the institute; an institute admin is fixed to their own.
            int? targetInstituteId = isSuperAdmin ? model.InstituteId : instituteId;

            if (targetInstituteId is null)
            {
                ModelState.AddModelError(
                    isSuperAdmin ? nameof(model.InstituteId) : string.Empty,
                    isSuperAdmin ? MsgKey.Validation.Required(Key.Institute) : "Your account is not linked to an institute.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateOptionsAsync(model, isSuperAdmin);
                return View(model);
            }

            Student student = new()
            {
                InstituteId = targetInstituteId!.Value,
                GradeId = model.GradeId,
                FullName = model.FullName.Trim(),
                RollNumber = model.RollNumber?.Trim(),
                Section = model.Section?.Trim(),
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                GuardianName = model.GuardianName?.Trim(),
                PhoneNumber = model.PhoneNumber?.Trim()
            };

            await _students.AddAsync(student);
            await _students.SaveChangesAsync();

            TempData["Success"] = MsgKey.Success.Created(Key.Student);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>Resolves whether the current user is a super admin and their institute (if any).</summary>
        private (bool isSuperAdmin, int? instituteId) GetScope()
            => (User.IsInRole(Roles.SuperAdmin), User.GetInstituteId());

        /// <summary>Fills the class dropdown (and, for super admins, the institute dropdown).</summary>
        private async Task PopulateOptionsAsync(CreateStudentViewModel model, bool isSuperAdmin)
        {
            model.GradeOptions = await _grades.Query()
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToListAsync();

            model.ShowInstitute = isSuperAdmin;

            if (isSuperAdmin)
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
