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
    public class ClassService(
        IRepository<InstituteClass> classes,
        IRepository<Section> sections,
        ITenantProvider tenant) : IClassService
    {
        private readonly IRepository<InstituteClass> _classes = classes;
        private readonly IRepository<Section> _sections = sections;
        private readonly ITenantProvider _tenant = tenant;

        public async Task<ClassIndexViewModel> GetIndexAsync(PageParameters tableParams)
        {
            ClassIndexViewModel model = new()
            {
                IsSuperAdmin = _tenant.IsSuperAdmin
            };

            IQueryable<InstituteClass> query = _classes.Query();

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                query = query.Where(c => c.Name.Contains(term)
                                      || c.Section.Name.Contains(term)
                                      || c.Institute.Name.Contains(term));
            }

            IQueryable<ClassRow> rows = query.Select(c => new ClassRow
            {
                Id = c.Id,
                Name = c.Name,
                SectionName = c.Section.Name,
                InstituteName = c.Institute.Name,
                IsActive = c.IsActive
            });

            string orderBy = tableParams.OrderBy;

            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "Name asc";
            }

            rows = rows.OrderBy(orderBy);

            model.Classes = await _classes.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);
            model.SectionOptions = await GetSectionOptionsAsync();

            return model;
        }

        public async Task<ClassFormViewModel?> GetFormAsync(int? id)
        {
            ClassFormViewModel model = new()
            {
                SectionOptions = await GetSectionOptionsAsync()
            };

            if (id is int classId)
            {
                InstituteClass? cls = await _classes.Query().FirstOrDefaultAsync(c => c.Id == classId && c.IsActive);

                if (cls is null)
                {
                    return null;
                }

                model.Id = cls.Id;
                model.Name = cls.Name;
                model.SectionId = cls.SectionId;
            }

            return model;
        }

        public async Task<bool> CreateOrUpdate(int? id, string name, int? sectionId)
        {
            if (string.IsNullOrWhiteSpace(name) || sectionId is null)
            {
                return false;
            }

            Section? section = await _sections.Query().FirstOrDefaultAsync(s => s.Id == sectionId && s.IsActive);

            if (section is null)
            {
                return false;
            }

            if (id is int classId)
            {
                InstituteClass? cls = await _classes.Query().FirstOrDefaultAsync(c => c.Id == classId && c.IsActive);

                if (cls is null)
                {
                    return false;
                }

                cls.Name = name.Trim();
                cls.SectionId = section.Id;
                cls.InstituteId = section.InstituteId;

                _classes.Update(cls);
                await _classes.SaveChangesAsync();
                return true;
            }

            await _classes.AddAsync(new InstituteClass
            {
                Name = name.Trim(),
                SectionId = section.Id,
                InstituteId = section.InstituteId
            });

            await _classes.SaveChangesAsync();
            return true;
        }

        // Sections available as class parents (own institute's, or all for a SuperAdmin).
        private async Task<IReadOnlyList<SelectListItem>> GetSectionOptionsAsync()
            => await _sections.Query()
                              .Where(s => s.IsActive)
                              .OrderBy(s => s.Name)
                              .Select(s => new SelectListItem
                              {
                                  Value = s.Id.ToString(),
                                  Text = s.Name
                              })
                              .ToListAsync();
    }
}
