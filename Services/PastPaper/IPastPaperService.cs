using Scholar.Models;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IPastPaperService
    {
        Task<List<Board>> GetBoardsAsync();

        Task<List<LookupOption>> GetGradesForBoardAsync(int boardId);

        Task<List<LookupOption>> GetSubjectsAsync(int boardId, int gradeId);

        Task<List<PastPaper>> GetPapersForSubjectAsync(int subjectId);

        Task<ManagePastPapersViewModel> BuildManageViewModelAsync(UploadPastPaperViewModel upload);

        Task<bool> SubjectExistsAsync(int subjectId);

        /// <summary>Stores the file, saves the record, and returns the paper's title.</summary>
        Task<string> SaveUploadAsync(UploadPastPaperViewModel model);

        Task<bool> DeleteAsync(int id);
    }

    /// <summary>A minimal {id,name} pair for cascading dropdown JSON feeds.</summary>
    public sealed record LookupOption(int Id, string Name);
}
