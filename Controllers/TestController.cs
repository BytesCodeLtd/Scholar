using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Constants;
using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly ITestService _tests;
        private readonly ILogger<TestController> _logger;

        public TestController(ITestService tests, ILogger<TestController> logger)
        {
            _tests = tests;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null)
        {
            ViewBag.IsSuperAdmin = _tests.IsSuperAdmin;
            ViewBag.Search = search;

            List<PaperCard> papers = await _tests.GetPapersAsync(search);
            return View(papers);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? search = null)
        {
            ViewBag.IsSuperAdmin = _tests.IsSuperAdmin;
            ViewBag.Search = search;

            List<PaperCard> papers = await _tests.GetPapersAsync(search);
            return PartialView("_PaperGrid", papers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int[] selectedTopicIds, int subjectId, int? instituteId)
        {
            PaperBuilderViewModel? vm = await _tests.BuildPaperFormAsync(selectedTopicIds, subjectId, instituteId);

            if (vm is null)
            {
                TempData["Error"] = _tests.IsSuperAdmin
                    ? "Select an institute to generate a paper for."
                    : "Your account isn't linked to an institute, so papers can't be branded.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> SearchQuestions(int[] topicIds, QuestionType type, QuestionCategory[] categories)
        {
            List<Question> questions = await _tests.SearchQuestionsAsync(topicIds, type, categories);
            return PartialView("_QuestionPicker", questions);
        }

        [HttpPost]
        public async Task<IActionResult> RenderSections([FromBody] RenderSectionsRequest request)
        {
            PaperSectionsViewModel? vm = await _tests.RenderSectionsAsync(request);
            return vm is null ? Forbid() : PartialView("_PaperSectionsList", vm);
        }

        [HttpPost]
        public async Task<IActionResult> RenderCanvas([FromBody] RenderCanvasRequest request)
        {
            PaperDocumentViewModel? vm = await _tests.RenderCanvasAsync(request);
            return vm is null ? Forbid() : PartialView("_PaperCanvas", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SavePaperRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Title))
            {
                return BadRequest(new { message = "Give the paper a title before saving." });
            }

            if (!(request.Sections ?? []).Any(s => s.QuestionIds is { Count: > 0 }))
            {
                return BadRequest(new { message = "Add at least one question section before saving." });
            }

            int? id = await _tests.SaveAsync(request);
            if (id is null)
            {
                _logger.LogWarning("Save paper blocked: no institute branding available.");
                return Forbid();
            }

            _logger.LogInformation("Paper {Id} saved ({Title}).", id, request.Title);
            return Ok(new { id, redirectUrl = Url.Action(nameof(Print), new { id }) });
        }

        [HttpGet]
        public async Task<IActionResult> Print(int id)
        {
            PaperDocumentViewModel? vm = await _tests.GetPrintModelAsync(id);
            return vm is null ? NotFound() : View(vm);
        }
    }
}
