using System.Linq.Dynamic.Core;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class TeacherService(IRepository<Teacher> teachers, ITenantProvider tenant) : ITeacherService
    {
        private readonly IRepository<Teacher> _teachers = teachers;
        private readonly ITenantProvider _tenant = tenant;

        public async Task<Common.Paging.PagedResult<TeacherRow>> GetTeachersAsync(PageParameters tableParams)
        {
            IQueryable<Teacher> teachers = _teachers.Query().Where(t => t.IsActive);

            if (!_tenant.IsSuperAdmin)
            {
                teachers = teachers.Where(t => t.User.InstituteId == _tenant.InstituteId);
            }

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                teachers = teachers.Where(t =>
                    (t.User.FullName != null && t.User.FullName.Contains(term)) ||
                    (t.User.Email != null && t.User.Email.Contains(term)) ||
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

            return await _teachers.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);
        }
    }
}
