using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Models.ViewModels;
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

            ClassIndexViewModel? model = await _classes.GetIndexAsync(tableParams ?? new PageParameters());

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdate(int? id)
        {
            ClassFormViewModel? model = await _classes.GetFormAsync(id);

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
            TempData["Success"] = id is null ? MsgKey.Success.Created(Key.Class) : MsgKey.Success.Updated(Key.Class);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        { 
            bool deleted = await _classes.DeleteAsync(id);

            if (deleted)
            {
                _logger.LogDebug("Deleted class (id {Id}).", id);
                TempData["Success"] = MsgKey.Success.Deleted(Key.Class);
            }
            else
            {
                TempData["Error"] = MsgKey.Error.DeleteFailed(Key.Class);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
