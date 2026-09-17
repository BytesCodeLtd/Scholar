using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class ClassController(IClassService classes, ILogger<ClassController> logger) : Controller
    {
        private readonly IClassService _classes = classes;
        private readonly ILogger<ClassController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters? tableParams = null)
        {
            _logger.LogDebug("Loading classes list (page {Page}).", tableParams?.Page ?? 1);

            return View(await _classes.GetIndexAsync(tableParams ?? new PageParameters()));
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdate(int? id)
        {
            Models.ViewModels.ClassFormViewModel? model = await _classes.GetFormAsync(id);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(int? id, string name, int? sectionId)
        {
            if (!await _classes.CreateOrUpdate(id, name, sectionId))
            {
                TempData["Error"] = "Class name and a section are required.";

                return id is null
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(CreateOrUpdate), new { id });
            }

            _logger.LogDebug("Saved class (id {Id}).", id);
            TempData["Success"] = id is null ? "Class added." : "Class updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
