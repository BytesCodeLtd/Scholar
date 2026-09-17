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
    public class SubjectGroupService(
        IRepository<SubjectGroup> groups,
        IRepository<InstituteClass> classes,
        IRepository<Section> sections,
        IRepository<InstituteSubject> subjects,
        IRepository<Institute> institutes,
        ITenantProvider tenant) : ISubjectGroupService
    {
        private readonly IRepository<SubjectGroup> _groups = groups;
        private readonly IRepository<InstituteClass> _classes = classes;
        private readonly IRepository<Section> _sections = sections;
        private readonly IRepository<InstituteSubject> _subjects = subjects;
        private readonly IRepository<Institute> _institutes = institutes;
        private readonly ITenantProvider _tenant = tenant;

        public async Task<SubjectGroupIndexViewModel> GetIndexAsync(PageParameters tableParams)
        {
            SubjectGroupIndexViewModel model = new()
            {
                IsSuperAdmin = _tenant.IsSuperAdmin
            };

            IQueryable<SubjectGroup> query = _groups.Query();

            if (!string.IsNullOrWhiteSpace(tableParams.Search))
            {
                string term = tableParams.Search;
                query = query.Where(g => g.Name.Contains(term)
                                      || g.Class.Name.Contains(term)
                                      || g.Institute.Name.Contains(term));
            }

            IQueryable<SubjectGroupRow> rows = query.Select(g => new SubjectGroupRow
            {
                Id = g.Id,
                Name = g.Name,
                ClassName = g.Class.Name,
                Sections = g.Sections.OrderBy(s => s.Name).Select(s => s.Name).ToList(),
                Subjects = g.Subjects.OrderBy(s => s.Name).Select(s => s.Name).ToList(),
                Description = g.Description,
                InstituteName = g.Institute.Name,
                IsActive = g.IsActive
            });

            string orderBy = tableParams.OrderBy;

            if (string.IsNullOrWhiteSpace(orderBy) || orderBy.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = "Name asc";
            }
            rows = rows.OrderBy(orderBy);

            model.Groups = await _groups.GetPagedAsync(rows, tableParams.Page, tableParams.PageSize);

            model.ClassOptions = await GetClassOptionsAsync();
            model.SectionOptions = await GetSectionOptionsAsync();
            model.SubjectOptions = await GetSubjectOptionsAsync();

            if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await GetInstituteOptionsAsync();
            }

            return model;
        }

        public async Task<SubjectGroupFormViewModel?> GetFormAsync(int? id)
        {
            SubjectGroupFormViewModel model = new()
            {
                IsSuperAdmin = _tenant.IsSuperAdmin,
                ClassOptions = await GetClassOptionsAsync(),
                SectionOptions = await GetSectionOptionsAsync(),
                SubjectOptions = await GetSubjectOptionsAsync()
            };

            if (id is int groupId)
            {
                SubjectGroup? group = await _groups.Query()
                    .Include(g => g.Sections)
                    .Include(g => g.Subjects)
                    .FirstOrDefaultAsync(g => g.Id == groupId && g.IsActive);

                if (group is null)
                {
                    return null;
                }

                model.Id = group.Id;
                model.Name = group.Name;
                model.Description = group.Description;
                model.ClassId = group.ClassId;
                model.InstituteId = group.InstituteId;
                model.SelectedSectionIds = group.Sections.Select(s => s.Id).ToHashSet();
                model.SelectedSubjectIds = group.Subjects.Select(s => s.Id).ToHashSet();
            }
            else if (_tenant.IsSuperAdmin)
            {
                model.InstituteOptions = await GetInstituteOptionsAsync();
            }

            return model;
        }

        public async Task<bool> CreateOrUpdate(int? id, string name, string? description, int? classId,
                                               int[] sectionIds, int[] subjectIds, int? instituteId)
        {
            if (string.IsNullOrWhiteSpace(name) || classId is null)
            {
                return false;
            }

            // Update path: an id was supplied. Institute is fixed to the group's own.
            if (id is int groupId)
            {
                SubjectGroup? group = await _groups.Query()
                    .Include(g => g.Sections)
                    .Include(g => g.Subjects)
                    .FirstOrDefaultAsync(g => g.Id == groupId && g.IsActive);

                if (group is null)
                {
                    return false;
                }

                Selection? sel = await ResolveSelections(group.InstituteId, classId.Value, sectionIds, subjectIds);

                if (sel is null)
                {
                    return false;
                }

                group.Name = name.Trim();
                group.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
                group.ClassId = sel.Class.Id;

                group.Sections.Clear();
                foreach (Section s in sel.Sections) group.Sections.Add(s);

                group.Subjects.Clear();
                foreach (InstituteSubject s in sel.Subjects) group.Subjects.Add(s);

                await _groups.SaveChangesAsync();
                return true;
            }

            int? targetInstituteId = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

            if (targetInstituteId is null)
            {
                return false;
            }

            Selection? newSel = await ResolveSelections(targetInstituteId.Value, classId.Value, sectionIds, subjectIds);

            if (newSel is null)
            {
                return false;
            }

            SubjectGroup group2 = new()
            {
                Name = name.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                InstituteId = targetInstituteId.Value,
                ClassId = newSel.Class.Id
            };

            foreach (Section s in newSel.Sections) group2.Sections.Add(s);
            foreach (InstituteSubject s in newSel.Subjects) group2.Subjects.Add(s);

            await _groups.AddAsync(group2);
            await _groups.SaveChangesAsync();

            return true;
        }

        public Task<bool> DeleteAsync(int id) => _groups.DeleteAsync(id);

        /// <summary>The resolved, tenant-validated class/sections/subjects for a save.</summary>
        private sealed record Selection(InstituteClass Class, List<Section> Sections, List<InstituteSubject> Subjects);

        /// <summary>Resolves and validates the class, sections and subjects against a single
        /// institute. Returns null if the class is invalid or nothing is selected in either list.</summary>
        private async Task<Selection?> ResolveSelections(int instituteId, int classId, int[] sectionIds, int[] subjectIds)
        {
            InstituteClass? cls = await _classes.Query()
                .FirstOrDefaultAsync(c => c.Id == classId && c.InstituteId == instituteId && c.IsActive);

            List<Section> selSections = await _sections.Query()
                .Where(s => sectionIds.Contains(s.Id) && s.InstituteId == instituteId && s.IsActive)
                .ToListAsync();

            List<InstituteSubject> selSubjects = await _subjects.Query()
                .Where(s => subjectIds.Contains(s.Id) && s.InstituteId == instituteId && s.IsActive)
                .ToListAsync();

            if (cls is null || selSections.Count == 0 || selSubjects.Count == 0)
            {
                return null;
            }

            return new Selection(cls, selSections, selSubjects);
        }

        private async Task<IReadOnlyList<SubjectGroupOption>> GetClassOptionsAsync()
            => await _classes.Query()
                             .Where(c => c.IsActive)
                             .OrderBy(c => c.Name)
                             .Select(c => new SubjectGroupOption { Id = c.Id, Name = c.Name, InstituteId = c.InstituteId })
                             .ToListAsync();

        private async Task<IReadOnlyList<SubjectGroupOption>> GetSectionOptionsAsync()
            => await _sections.Query()
                              .Where(s => s.IsActive)
                              .OrderBy(s => s.Name)
                              .Select(s => new SubjectGroupOption { Id = s.Id, Name = s.Name, InstituteId = s.InstituteId })
                              .ToListAsync();

        private async Task<IReadOnlyList<SubjectGroupOption>> GetSubjectOptionsAsync()
            => await _subjects.Query()
                              .Where(s => s.IsActive)
                              .OrderBy(s => s.Name)
                              .Select(s => new SubjectGroupOption { Id = s.Id, Name = s.Name, InstituteId = s.InstituteId })
                              .ToListAsync();

        private async Task<IReadOnlyList<SelectListItem>> GetInstituteOptionsAsync()
            => await _institutes.Query()
                                .Where(i => i.IsActive)
                                .OrderBy(i => i.Name)
                                .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                                .ToListAsync();
    }
}
