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
    public class InstituteSubjectService(
        IRepository<InstituteSubject> subjects,
        IRepository<Institute> institutes,
        ITenantProvider tenant) : IInstituteSubjectService
    {
        private readonly IRepository<InstituteSubject> _subjects = subjects;
        private readonly IRepository<Institute> _institutes = institutes;
        private readonly ITenantProvider _tenant = tenant;

        public async Task<InstituteSubjectIndexViewModel> GetIndexAsync(PageParameters tableParams)
        {
            InstituteSubjectIndexViewModel model = new()
            {
                IsSuperAdmin = _tenant.IsSuperAdmin
            };

            IQueryable<InstituteSubject> query = _subjects.Query();

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                query = query.Where(s => s.Name.Contains(term)
                                      || s.Code.Contains(term)
                                      || s.Institute.Name.Contains(term));
            }

            IQueryable<InstituteSubjectRow> rows = query.Select(s => new InstituteSubjectRow
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Type = s.Type,
                InstituteName = s.Institute.Name,
                IsActive = s.IsActive
            });

            string orderBy = tableParams.OrderBy;

            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "Name asc";
            }
            rows = rows.OrderBy(orderBy);

            model.Subjects = await _subjects.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);

            if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await GetInstituteOptionsAsync();
            }

            return model;
        }

        public async Task<InstituteSubjectFormViewModel?> GetFormAsync(int? id)
        {
            InstituteSubjectFormViewModel model = new() { IsSuperAdmin = _tenant.IsSuperAdmin };

            if (id is int subjectId)
            {
                InstituteSubject? subject = await _subjects.Query().FirstOrDefaultAsync(s => s.Id == subjectId && s.IsActive);

                if (subject is null)
                {
                    return null;
                }

                model.Id = subject.Id;
                model.Name = subject.Name;
                model.Code = subject.Code;
                model.Type = subject.Type;
            }
            else if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await GetInstituteOptionsAsync();
            }

            return model;
        }

        public async Task<bool> CreateOrUpdate(int? id, string name, string code, SubjectType type, int? instituteId)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            // Update path: an id was supplied.
            if (id is int subjectId)
            {
                InstituteSubject? subject = await _subjects.Query().FirstOrDefaultAsync(s => s.Id == subjectId && s.IsActive);

                if (subject is null)
                {
                    return false;
                }

                subject.Name = name.Trim();
                subject.Code = code.Trim();
                subject.Type = type;

                _subjects.Update(subject);
                await _subjects.SaveChangesAsync();

                return true;
            }

            int? targetInstituteId = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

            if (targetInstituteId is null)
            {
                return false;
            }

            await _subjects.AddAsync(new InstituteSubject
            {
                Name = name.Trim(),
                Code = code.Trim(),
                Type = type,
                InstituteId = targetInstituteId.Value
            });

            await _subjects.SaveChangesAsync();

            return true;
        }

        public Task<bool> DeleteAsync(int id) => _subjects.DeleteAsync(id);

        private async Task<IReadOnlyList<SelectListItem>> GetInstituteOptionsAsync()
            => await _institutes.Query()
                                .Where(i => i.IsActive)
                                .OrderBy(i => i.Name)
                                .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                                .ToListAsync();
    }
}
