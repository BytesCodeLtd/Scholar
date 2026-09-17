using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboard;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboard, ILogger<DashboardController> logger)
        {
            _dashboard = dashboard;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = await _dashboard.BuildDashboardAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLayout(string? layout)
        {
            if (layout is { Length: > 4000 })
            {
                return BadRequest();
            }

            bool saved = await _dashboard.SaveLayoutAsync(string.IsNullOrWhiteSpace(layout) ? null : layout);
            if (!saved)
            {
                _logger.LogWarning("Dashboard layout save found nothing to update.");
            }

            return saved ? Ok() : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetLayout()
        {
            bool saved = await _dashboard.SaveLayoutAsync(null);
            if (saved)
            {
                _logger.LogInformation("Dashboard layout reset.");
            }

            return saved ? Ok() : NotFound();
        }
    }
}
