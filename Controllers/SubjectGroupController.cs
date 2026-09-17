using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class SubjectGroupController(ISubjectGroupService groups, ILogger<SubjectGroupController> logger) : Controller
    {
        private readonly ISubjectGroupService _groups = groups;
        private readonly ILogger<SubjectGroupController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters? tableParams = null)
        {
            _logger.LogDebug("Loading subject groups list (page {Page}).", tableParams?.Page ?? 1);

            return View(await _groups.GetIndexAsync(tableParams ?? new PageParameters()));
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdate(int? id)
        {
            Models.ViewModels.SubjectGroupFormViewModel? model = await _groups.GetFormAsync(id);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(
            int? id, string name, string? description, int? classId,
            int[] sectionIds, int[] subjectIds, int? instituteId)
        {
            if (!await _groups.CreateOrUpdate(id, name, description, classId, sectionIds ?? [], subjectIds ?? [], instituteId))
            {
                TempData["Error"] = id is null
                    ? "Name, class, and at least one section and subject are required (an institute must be selected)."
                    : "Name, class, and at least one section and subject are required.";

                return id is null
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(CreateOrUpdate), new { id });
            }

            _logger.LogDebug("Saved subject group (id {Id}).", id);
            TempData["Success"] = id is null ? MsgKey.Success.Created(Key.SubjectGroup) : MsgKey.Success.Updated(Key.SubjectGroup);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _groups.DeleteAsync(id))
            {
                _logger.LogDebug("Deleted subject group (id {Id}).", id);
                TempData["Success"] = MsgKey.Success.Deleted(Key.SubjectGroup);
            }
            else
            {
                TempData["Error"] = MsgKey.Error.DeleteFailed(Key.SubjectGroup);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
