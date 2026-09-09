using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Constants;
using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private const int ChartMonths = 6;
        private readonly IRepository<Test> _testRepository;
        private readonly IRepository<Institute> _instituteRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Teacher> _teacherRepository;
        private readonly IRepository<PastPaper> _pastPaperRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            IRepository<Test> test,
            IRepository<Institute> institute,
            IRepository<Question> question,
            IRepository<Student> student,
            IRepository<Teacher> teacher,
            IRepository<PastPaper> pastPaper,
            UserManager<ApplicationUser> userManager)
        {
            _testRepository = test;
            _instituteRepository = institute;
            _questionRepository = question;
            _studentRepository = student;
            _teacherRepository = teacher;
            _pastPaperRepository = pastPaper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            bool isSuperAdmin = User.IsInRole(Roles.SuperAdmin);
            int? instituteId = User.GetInstituteId();

            DashboardViewModel model = new();

            DateTime monthStart = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime nextMonthStart = monthStart.AddMonths(1);

            IQueryable<Test> tests = _testRepository.Query();

            if (!isSuperAdmin)
            {
                tests = tests.Where(t => t.InstituteId == instituteId);
            }

            var testStats = await tests.GroupBy(_ => 1)
                                       .Select(g => new
                                       {
                                           Total = g.Count(),
                                           Saved = g.Count(t => t.IsActive),
                                           ThisMonth = g.Count(t => t.CreatedAt >= monthStart && t.CreatedAt < nextMonthStart)
                                       })
                                       .FirstOrDefaultAsync();

            model.TotalTests = testStats?.Total ?? 0;
            model.SavedTests = testStats?.Saved ?? 0;
            model.TestsThisMonth = testStats?.ThisMonth ?? 0;

            DateTime testFirstMonth = monthStart.AddMonths(-(ChartMonths - 1));

            var testsMonthly = await tests.Where(t => t.CreatedAt >= testFirstMonth && t.CreatedAt < nextMonthStart)
                                          .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                                          .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                                          .ToListAsync();

            for (int i = 0; i < ChartMonths; i++)
            {
                DateTime m = testFirstMonth.AddMonths(i);
                model.TestChartLabels.Add(m.ToString("MMM"));
                model.TestChartData.Add(testsMonthly.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Count ?? 0);
            }

            Dictionary<QuestionType, int> questionsByType = await _questionRepository.Query()
                                                                                     .Where(q => q.IsActive)
                                                                                     .GroupBy(q => q.Type)
                                                                                     .Select(g => new { Type = g.Key, Count = g.Count() })
                                                                                     .ToDictionaryAsync(x => x.Type, x => x.Count);

            model.TotalMcqs = questionsByType.GetValueOrDefault(QuestionType.Mcq);
            model.TotalShortQuestions = questionsByType.GetValueOrDefault(QuestionType.Short);
            model.TotalLongQuestions = questionsByType.GetValueOrDefault(QuestionType.Long);

            IQueryable<Student> students = _studentRepository.Query().Where(s => s.IsActive);

            if (!isSuperAdmin)
            {
                students = students.Where(s => s.InstituteId == instituteId);
            }

            var studentStats = await students.GroupBy(_ => 1)
                                             .Select(g => new
                                             {
                                                 Total = g.Count(),
                                                 ThisMonth = g.Count(s => s.CreatedAt >= monthStart && s.CreatedAt < nextMonthStart)
                                             })
                                             .FirstOrDefaultAsync();

            model.TotalStudents = studentStats?.Total ?? 0;
            model.StudentsThisMonth = studentStats?.ThisMonth ?? 0;

            IQueryable<Teacher> teachers = _teacherRepository.Query().Where(t => t.IsActive);

            if (!isSuperAdmin)
            {
                teachers = teachers.Where(t => t.User.InstituteId == instituteId);
            }

            model.TotalTeachers = await teachers.CountAsync();

            var pastPapersByBoard = await _pastPaperRepository.Query()
                                                              .Where(p => p.IsActive)
                                                              .GroupBy(p => p.Subject.Board.Name)
                                                              .Select(g => new { Board = g.Key, Count = g.Count() })
                                                              .OrderByDescending(x => x.Count)
                                                              .ToListAsync();

            foreach (var b in pastPapersByBoard)
            {
                model.PastPaperChartLabels.Add(b.Board);
                model.PastPaperChartData.Add(b.Count);
            }

            model.TotalPastPapers = model.PastPaperChartData.Sum();

            if (isSuperAdmin)
            {
                model.IsSuperAdmin = true;

                DateTime firstMonth = monthStart.AddMonths(-(ChartMonths - 1));

                var instituteStats = await _instituteRepository.Query()
                                                               .GroupBy(_ => 1)
                                                               .Select(g => new
                                                               {
                                                                   Total = g.Count(),
                                                                   Active = g.Count(i => i.IsActive),
                                                                   ThisMonth = g.Count(i => i.CreatedAt >= monthStart && i.CreatedAt < nextMonthStart),
                                                                   Baseline = g.Count(i => i.CreatedAt < firstMonth)
                                                               })
                                                               .FirstOrDefaultAsync();

                model.TotalInstitutes = instituteStats?.Total ?? 0;
                model.ActiveInstitutes = instituteStats?.Active ?? 0;
                model.InstitutesThisMonth = instituteStats?.ThisMonth ?? 0;

                int baseline = instituteStats?.Baseline ?? 0;

                var monthlyNew = await _instituteRepository.Query()
                                                           .Where(i => i.CreatedAt >= firstMonth && i.CreatedAt < nextMonthStart)
                                                           .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                                                           .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                                                           .ToListAsync();

                int running = baseline;
                for (int i = 0; i < ChartMonths; i++)
                {
                    DateTime m = firstMonth.AddMonths(i);
                    running += monthlyNew.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Count ?? 0;
                    model.InstituteChartLabels.Add(m.ToString("MMM"));
                    model.InstituteChartData.Add(running);
                }
            }

            string? layoutUserId = _userManager.GetUserId(User);
            model.LayoutJson = instituteId is int layoutInstituteId
                ? await _instituteRepository.Query()
                                            .AsNoTracking()
                                            .Where(i => i.Id == layoutInstituteId)
                                            .Select(i => i.DashboardLayout)
                                            .FirstOrDefaultAsync()
                : await _userManager.Users
                                    .AsNoTracking()
                                    .Where(u => u.Id == layoutUserId)
                                    .Select(u => u.DashboardLayout)
                                    .FirstOrDefaultAsync();

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

            return await PersistLayoutAsync(string.IsNullOrWhiteSpace(layout) ? null : layout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ResetLayout() => PersistLayoutAsync(null);

        private async Task<IActionResult> PersistLayoutAsync(string? layout)
        {
            int? instituteId = User.GetInstituteId();

            if (instituteId is int iid)
            {
                int rows = await _instituteRepository.Query()
                                                     .Where(i => i.Id == iid)
                                                     .ExecuteUpdateAsync(s => s.SetProperty(i => i.DashboardLayout, layout)
                                                                               .SetProperty(i => i.UpdatedAt, DateTime.UtcNow));

                return rows == 0 ? NotFound() : Ok();
            }

            string? userId = _userManager.GetUserId(User);

            if (userId is null)
            {
                return Unauthorized();
            }

            await _userManager.Users.Where(u => u.Id == userId)
                                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.DashboardLayout, layout));

            return Ok();
        }
    }
}
