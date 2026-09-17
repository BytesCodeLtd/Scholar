using Microsoft.EntityFrameworkCore;
using Scholar.Common.Storage;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class PastPaperService(
        IRepository<PastPaper> pastPapers,
        IRepository<Board> boards,
        IRepository<Grade> grades,
        IRepository<Subject> subjects,
        IFileStorage fileStorage) : IPastPaperService
    {
        private readonly IRepository<PastPaper> _pastPapers = pastPapers;
        private readonly IRepository<Board> _boards = boards;
        private readonly IRepository<Grade> _grades = grades;
        private readonly IRepository<Subject> _subjects = subjects;
        private readonly IFileStorage _fileStorage = fileStorage;

        public Task<List<Board>> GetBoardsAsync()
            => _boards.Query().AsNoTracking().OrderBy(b => b.Name).ToListAsync();

        public Task<List<LookupOption>> GetGradesForBoardAsync(int boardId)
            => _grades.Query()
                      .AsNoTracking()
                      .Where(g => g.Subjects.Any(s => s.BoardId == boardId))
                      .OrderBy(g => g.Id)
                      .Select(g => new LookupOption(g.Id, g.Name))
                      .ToListAsync();

        public Task<List<LookupOption>> GetSubjectsAsync(int boardId, int gradeId)
            => _subjects.Query()
                        .AsNoTracking()
                        .Where(s => s.BoardId == boardId && s.GradeId == gradeId)
                        .OrderBy(s => s.Name)
                        .Select(s => new LookupOption(s.Id, s.Name))
                        .ToListAsync();

        public Task<List<PastPaper>> GetPapersForSubjectAsync(int subjectId)
            => _pastPapers.Query()
                         .AsNoTracking()
                         .Where(p => p.IsActive && p.SubjectId == subjectId)
                         .OrderByDescending(p => p.Year)
                         .ThenBy(p => p.Title)
                         .ToListAsync();

        public async Task<ManagePastPapersViewModel> BuildManageViewModelAsync(UploadPastPaperViewModel upload)
        {
            List<Board> boards = await GetBoardsAsync();

            return new ManagePastPapersViewModel
            {
                Boards = boards,
                Upload = upload
            };
        }

        public Task<bool> SubjectExistsAsync(int subjectId)
            => subjectId > 0 ? _subjects.Query().AnyAsync(s => s.Id == subjectId) : Task.FromResult(false);

        public async Task<string> SaveUploadAsync(UploadPastPaperViewModel model)
        {
            string fileUrl = await _fileStorage.UploadAsync(model.File!, "past-papers");

            PastPaper paper = new()
            {
                SubjectId = model.SubjectId,
                Title = model.Title.Trim(),
                Year = model.Year,
                FileUrl = fileUrl,
                OriginalFileName = Path.GetFileName(model.File!.FileName)
            };

            await _pastPapers.AddAsync(paper);
            await _pastPapers.SaveChangesAsync();

            return paper.Title;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            PastPaper? paper = await _pastPapers.GetByIdAsync(id);

            if (paper is null)
            {
                return false;
            }

            await _fileStorage.DeleteAsync(paper.FileUrl);
            _pastPapers.Remove(paper);
            await _pastPapers.SaveChangesAsync();

            return true;
        }
    }
}
