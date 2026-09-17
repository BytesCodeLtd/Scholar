using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Constants;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class PastPaperController : Controller
    {
        private readonly IPastPaperService _pastPapers;
        private readonly ILogger<PastPaperController> _logger;

        public PastPaperController(IPastPaperService pastPapers, ILogger<PastPaperController> logger)
        {
            _pastPapers = pastPapers;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Board> boards = await _pastPapers.GetBoardsAsync();
            return View(boards);
        }

        // Cascading dropdown feeds.
        [HttpGet]
        public async Task<IActionResult> Grades(int boardId)
            => Json(await _pastPapers.GetGradesForBoardAsync(boardId));

        [HttpGet]
        public async Task<IActionResult> Subjects(int boardId, int gradeId)
            => Json(await _pastPapers.GetSubjectsAsync(boardId, gradeId));

        [HttpGet]
        public async Task<IActionResult> List(int subjectId)
        {
            List<PastPaper> papers = await _pastPapers.GetPapersForSubjectAsync(subjectId);
            return PartialView("_PastPaperList", papers);
        }

        [HttpGet]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> Manage()
            => View(await _pastPapers.BuildManageViewModelAsync(new UploadPastPaperViewModel()));

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([Bind(Prefix = nameof(ManagePastPapersViewModel.Upload))] UploadPastPaperViewModel model)
        {
            ValidateFile(model.File);

            if (!await _pastPapers.SubjectExistsAsync(model.SubjectId))
            {
                ModelState.AddModelError($"{nameof(ManagePastPapersViewModel.Upload)}.{nameof(model.SubjectId)}", "Select a valid subject.");
            }

            if (!ModelState.IsValid)
            {
                return View(nameof(Manage), await _pastPapers.BuildManageViewModelAsync(model));
            }

            string title = await _pastPapers.SaveUploadAsync(model);

            _logger.LogInformation("Past paper \"{Title}\" uploaded for subject {SubjectId}.", title, model.SubjectId);
            TempData["Success"] = $"Past paper \"{title}\" uploaded.";
            return RedirectToAction(nameof(Manage));
        }

        // Validate the uploaded PDF (presence, type, size) straight onto ModelState.
        private void ValidateFile(IFormFile? file)
        {
            const long maxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
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

            if (file.Length > maxFileSizeBytes)
            {
                ModelState.AddModelError(key, Message.PastPaperTooLarge);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _pastPapers.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Delete failed: past paper {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Past paper {Id} deleted.", id);
            TempData["Success"] = "Past paper deleted.";
            return RedirectToAction(nameof(Manage));
        }
    }
}
