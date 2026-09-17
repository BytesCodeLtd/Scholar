using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Common.Paging;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class SectionService(
        IRepository<Section> sections,
        IRepository<Institute> institutes,
        ITenantProvider tenant) : ISectionService
    {
        private readonly IRepository<Section> _sections = sections;
        private readonly IRepository<Institute> _institutes = institutes;
        private readonly ITenantProvider _tenant = tenant;

        public async Task<SectionIndexViewModel> GetIndexAsync(PageParameters tableParams)
        {
            SectionIndexViewModel model = new()
            {
                IsSuperAdmin = _tenant.IsSuperAdmin
            };

            IQueryable<Section> query = _sections.Query();

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                query = query.Where(s => s.Name.Contains(term) || s.Institute.Name.Contains(term));
            }

            IQueryable<SectionRow> rows = query.Select(s => new SectionRow
            {
                Id = s.Id,
                Name = s.Name,
                InstituteName = s.Institute.Name,
                IsActive = s.IsActive
            });

            string orderBy = tableParams.OrderBy;

            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "Name asc";
            }
            rows = rows.OrderBy(orderBy);

            model.Sections = await _sections.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);

            if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await GetInstituteOptionsAsync();
            }

            return model;
        }

        public async Task<SectionFormViewModel?> GetFormAsync(int? id)
        {
            SectionFormViewModel model = new() { IsSuperAdmin = _tenant.IsSuperAdmin };

            if (id is int sectionId)
            {
                Section? section = await _sections.Query().FirstOrDefaultAsync(s => s.Id == sectionId && s.IsActive);

                if (section is null)
                {
                    return null;
                }

                model.Id = section.Id;
                model.Name = section.Name;
            }
            else if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await GetInstituteOptionsAsync();
            }

            return model;
        }

        public async Task<bool> CreateOrUpdate(int? id, string name, int? instituteId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            // Update path: an id was supplied.
            if (id is int sectionId)
            {
                Section? section = await _sections.Query().FirstOrDefaultAsync(s => s.Id == sectionId && s.IsActive);

                if (section is null)
                {
                    return false;
                }

                section.Name = name.Trim();

                _sections.Update(section);
                await _sections.SaveChangesAsync();

                return true;
            }

            int? targetInstituteId = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

            if (targetInstituteId is null)
            {
                return false;
            }

            await _sections.AddAsync(new Section
            {
                Name = name.Trim(),
                InstituteId = targetInstituteId.Value
            });

            await _sections.SaveChangesAsync();

            return true;
        }

        private async Task<IReadOnlyList<SelectListItem>> GetInstituteOptionsAsync()
            => await _institutes.Query()
                                .Where(i => i.IsActive)
                                .OrderBy(i => i.Name)
                                .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                                .ToListAsync();
    }
}
