using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Enums;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class InstituteSubjectController(IInstituteSubjectService subjects, ILogger<InstituteSubjectController> logger) : Controller
    {
        private readonly IInstituteSubjectService _subjects = subjects;
        private readonly ILogger<InstituteSubjectController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters? tableParams = null)
        {
            _logger.LogDebug("Loading subjects list (page {Page}).", tableParams?.Page ?? 1);

            return View(await _subjects.GetIndexAsync(tableParams ?? new PageParameters()));
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdate(int? id)
        {
            Models.ViewModels.InstituteSubjectFormViewModel? model = await _subjects.GetFormAsync(id);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(int? id, string name, string code, SubjectType type, int? instituteId)
        {
            if (!await _subjects.CreateOrUpdate(id, name, code, type, instituteId))
            {
                TempData["Error"] = id is null
                    ? "Subject name and code are required, and an institute must be selected."
                    : "Subject name and code are required.";

                return id is null
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(CreateOrUpdate), new { id });
            }

            _logger.LogDebug("Saved subject (id {Id}).", id);
            TempData["Success"] = id is null ? MsgKey.Success.Created(Key.Subject) : MsgKey.Success.Updated(Key.Subject);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _subjects.DeleteAsync(id);

            if (deleted)
            {
                _logger.LogDebug("Deleted subject (id {Id}).", id);
                TempData["Success"] = MsgKey.Success.Deleted(Key.Subject);
            }
            else
            {
                TempData["Error"] = MsgKey.Error.DeleteFailed(Key.Subject);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
