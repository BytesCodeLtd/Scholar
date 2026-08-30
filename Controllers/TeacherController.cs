using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        private readonly IRepository<Teacher> _teachers;

        public TeacherController(IRepository<Teacher> teachers)
        {
            _teachers = teachers;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters tableParams = null)
        {
            tableParams ??= new PageParameters();

            IQueryable<Teacher> teachers = _teachers.Query().Where(t => t.IsActive);

            if (!User.IsInRole(Roles.SuperAdmin))
            {
                int? instituteId = User.GetInstituteId();
                teachers = teachers.Where(t => t.User.InstituteId == instituteId);
            }

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                teachers = teachers.Where(t =>
                    t.User.FullName.Contains(term) ||
                    t.User.Email.Contains(term) ||
                    t.Subject.Name.Contains(term) ||
                    t.Grade.Name.Contains(term));
            }

            IQueryable<TeacherRow> rows = teachers
                .GroupBy(t => t.UserId)
                .Select(g => new TeacherRow
                {
                    Id = g.Key,
                    Name = g.First().User.FullName,
                    Email = g.First().User.Email,
                    Subjects = string.Join(", ", g.Select(x => x.Subject.Name).Distinct()),
                    Classes = string.Join(", ", g.Select(x => x.Grade.Name).Distinct())
                });

            string orderBy = tableParams.OrderBy;
            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "Email asc";
            }
            rows = rows.OrderBy(orderBy);

            Scholar.Common.Paging.PagedResult<TeacherRow> paged = await _teachers.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);

            return View(paged);
        }
    }
}
