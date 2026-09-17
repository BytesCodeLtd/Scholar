using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teachers;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(ITeacherService teachers, ILogger<TeacherController> logger)
        {
            _teachers = teachers;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters? tableParams = null)
        {
            _logger.LogDebug("Loading teachers list (page {Page}).", tableParams?.Page ?? 1);
            PagedResult<TeacherRow> paged = await _teachers.GetTeachersAsync(tableParams ?? new PageParameters());
            return View(paged);
        }
    }
}
