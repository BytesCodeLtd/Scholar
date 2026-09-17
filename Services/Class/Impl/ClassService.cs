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
                                      || c.Sections.Any(s => s.Name.Contains(term))
                                      || c.Institute.Name.Contains(term));
            }

            IQueryable<ClassRow> rows = query.Select(c => new ClassRow
            {
                Id = c.Id,
                Name = c.Name,
                Sections = c.Sections.OrderBy(s => s.Name).Select(s => s.Name).ToList(),
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
                InstituteClass? cls = await _classes.Query()
                                                    .Include(c => c.Sections)
                                                    .FirstOrDefaultAsync(c => c.Id == classId && c.IsActive);

                if (cls is null)
                {
                    return null;
                }

                model.Id = cls.Id;
                model.Name = cls.Name;
                model.SelectedSectionIds = cls.Sections.Select(s => s.Id).ToHashSet();
            }

            return model;
        }

        public async Task<bool> CreateOrUpdate(int? id, string name, int[] sectionIds)
        {
            if (string.IsNullOrWhiteSpace(name) || sectionIds is null || sectionIds.Length == 0)
            {
                return false;
            }

            List<Section> selected = await _sections.Query()
                                                    .Where(s => sectionIds.Contains(s.Id) && s.IsActive)
                                                    .ToListAsync();

            // All chosen sections must belong to a single institute (which becomes the class's).
            if (selected.Count == 0 || selected.Select(s => s.InstituteId).Distinct().Count() != 1)
            {
                return false;
            }

            int instituteId = selected[0].InstituteId;

            if (id is int classId)
            {
                InstituteClass? cls = await _classes.Query()
                                                    .Include(c => c.Sections)
                                                    .FirstOrDefaultAsync(c => c.Id == classId && c.IsActive);

                if (cls is null)
                {
                    return false;
                }

                cls.Name = name.Trim();
                cls.InstituteId = instituteId;

                cls.Sections.Clear();
                foreach (Section s in selected) cls.Sections.Add(s);

                await _classes.SaveChangesAsync();
                return true;
            }

            InstituteClass created = new()
            {
                Name = name.Trim(),
                InstituteId = instituteId
            };

            foreach (Section s in selected) created.Sections.Add(s);

            await _classes.AddAsync(created);
            await _classes.SaveChangesAsync();
            return true;
        }

        public Task<bool> DeleteAsync(int id) => _classes.DeleteAsync(id);

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
