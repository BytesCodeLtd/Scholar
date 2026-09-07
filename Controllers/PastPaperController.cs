using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Storage;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class PastPaperController : Controller
    {
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        private readonly IRepository<PastPaper> _pastPapers;
        private readonly IRepository<Board> _boards;
        private readonly IRepository<Grade> _grades;
        private readonly IRepository<Subject> _subjects;
        private readonly IFileStorage _fileStorage;

        public PastPaperController(
            IRepository<PastPaper> pastPapers,
            IRepository<Board> boards,
            IRepository<Grade> grades,
            IRepository<Subject> subjects,
            IFileStorage fileStorage)
        {
            _pastPapers = pastPapers;
            _boards = boards;
            _grades = grades;
            _subjects = subjects;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Board> boards = await _boards.Query().AsNoTracking().OrderBy(b => b.Name).ToListAsync();

            return View(boards);
        }

        // Cascading dropdown feeds.
        [HttpGet]
        public async Task<IActionResult> Grades(int boardId)
        {
            // Grades are global; only surface those with subjects for the chosen board.
            var grades = await _grades.Query()
                                      .AsNoTracking()
                                      .Where(g => g.Subjects.Any(s => s.BoardId == boardId))
                                      .OrderBy(g => g.Id)
                                      .Select(g => new { g.Id, g.Name })
                                      .ToListAsync();
            return Json(grades);
        }

        [HttpGet]
        public async Task<IActionResult> Subjects(int boardId, int gradeId)
        {
            var subjects = await _subjects.Query()
                                          .AsNoTracking()
                                          .Where(s => s.BoardId == boardId && s.GradeId == gradeId)
                                          .OrderBy(s => s.Name)
                                          .Select(s => new { s.Id, s.Name })
                                          .ToListAsync();
            return Json(subjects);
        }

        [HttpGet]
        public async Task<IActionResult> List(int subjectId)
        {
            List<PastPaper> papers = await _pastPapers.Query()
                                                      .AsNoTracking()
                                                      .Where(p => p.IsActive && p.SubjectId == subjectId)
                                                      .OrderByDescending(p => p.Year)
                                                      .ThenBy(p => p.Title)
                                                      .ToListAsync();

            return PartialView("_PastPaperList", papers);
        }

        [HttpGet]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> Manage()
        {
            return View(await BuildManageViewModelAsync(new UploadPastPaperViewModel()));
        }

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([Bind(Prefix = nameof(ManagePastPapersViewModel.Upload))] UploadPastPaperViewModel model)
        {
            ValidateFile(model.File);

            bool subjectExists = model.SubjectId > 0 && await _subjects.Query().AnyAsync(s => s.Id == model.SubjectId);
            if (!subjectExists)
            {
                ModelState.AddModelError($"Upload.{nameof(model.SubjectId)}", "Select a valid subject.");
            }

            if (!ModelState.IsValid)
            {
                return View(nameof(Manage), await BuildManageViewModelAsync(model));
            }

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

            TempData["Success"] = $"Past paper \"{paper.Title}\" uploaded.";
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            PastPaper? paper = await _pastPapers.GetByIdAsync(id);
            if (paper is null)
            {
                return NotFound();
            }

            await _fileStorage.DeleteAsync(paper.FileUrl);
            _pastPapers.Remove(paper);
            await _pastPapers.SaveChangesAsync();

            TempData["Success"] = "Past paper deleted.";
            return RedirectToAction(nameof(Manage));
        }

        private async Task<ManagePastPapersViewModel> BuildManageViewModelAsync(UploadPastPaperViewModel upload)
        {
            List<Board> boards = await _boards.Query().AsNoTracking().OrderBy(b => b.Name).ToListAsync();

            return new ManagePastPapersViewModel
            {
                Boards = boards,
                Upload = upload
            };
        }

        private void ValidateFile(IFormFile? file)
        {
            string key = $"{nameof(ManagePastPapersViewModel.Upload)}.{nameof(UploadPastPaperViewModel.File)}";

            if (file is null || file.Length == 0)
            {
                ModelState.AddModelError(key, Message.PastPaperFileRequired);
                return;
            }

            if (!string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(key, Message.PastPaperInvalidType);
            }

            if (file.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError(key, Message.PastPaperTooLarge);
            }
        }
    }
}
