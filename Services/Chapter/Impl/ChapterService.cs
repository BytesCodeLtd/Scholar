using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Models;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class ChapterService(
        IRepository<Chapter> chapters,
        IRepository<Institute> institutes,
        ITenantProvider tenant) : IChapterService
    {
        private readonly IRepository<Chapter> _chapters = chapters;
        private readonly IRepository<Institute> _institutes = institutes;
        private readonly ITenantProvider _tenant = tenant;

        public bool IsSuperAdmin => _tenant.IsSuperAdmin;

        public Task<List<Chapter>> GetChaptersAsync(int subjectId)
            => _chapters.Query()
                        .Where(x => x.SubjectId == subjectId)
                        .Include(c => c.Topics)
                        .Include(c => c.Subject)
                        .OrderBy(c => c.Number)
                        .ToListAsync();

        public Task<List<Institute>> GetInstitutesAsync()
            => _institutes.Query().AsNoTracking().ToListAsync();
    }
}
