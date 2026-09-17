using Scholar.Models;

namespace Scholar.Services
{
    public interface IChapterService
    {
        bool IsSuperAdmin { get; }

        Task<List<Chapter>> GetChaptersAsync(int subjectId);

        /// <summary>Institutes for the super-admin picker.</summary>
        Task<List<Institute>> GetInstitutesAsync();
    }
}
