using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class SectionController(ISectionService sections, ILogger<SectionController> logger) : Controller
    {
        private readonly ISectionService _sections = sections;
        private readonly ILogger<SectionController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters? tableParams = null)
        {
            _logger.LogDebug("Loading sections list (page {Page}).", tableParams?.Page ?? 1);

            return View(await _sections.GetIndexAsync(tableParams ?? new PageParameters()));
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdate(int? id)
        {
            Models.ViewModels.SectionFormViewModel? model = await _sections.GetFormAsync(id);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(int? id, string name, int? instituteId)
        {
            if (!await _sections.CreateOrUpdate(id, name, instituteId))
            {
                TempData["Error"] = id is null
                    ? "Section name is required, and an institute must be selected."
                    : "Section name is required.";

                return id is null
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(CreateOrUpdate), new { id });
            }

            _logger.LogDebug("Saved section (id {Id}).", id);
            TempData["Success"] = id is null ? MsgKey.Success.Created(Key.Section) : MsgKey.Success.Updated(Key.Section);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _sections.DeleteAsync(id);

            if (deleted)
            {
                _logger.LogDebug("Deleted section (id {Id}).", id);
                TempData["Success"] = MsgKey.Success.Deleted(Key.Section);
            }
            else
            {
                TempData["Error"] = MsgKey.Error.DeleteFailed(Key.Section);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
