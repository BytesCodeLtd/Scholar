using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class DashboardService(
        IRepository<Test> tests,
        IRepository<Institute> institutes,
        IRepository<Question> questions,
        IRepository<Student> students,
        IRepository<Teacher> teachers,
        IRepository<PastPaper> pastPapers,
        UserManager<ApplicationUser> userManager,
        ITenantProvider tenant,
        IHttpContextAccessor http) : IDashboardService
    {
        private const int ChartMonths = 6;

        private readonly IRepository<Test> _tests = tests;
        private readonly IRepository<Institute> _institutes = institutes;
        private readonly IRepository<Question> _questions = questions;
        private readonly IRepository<Student> _students = students;
        private readonly IRepository<Teacher> _teachers = teachers;
        private readonly IRepository<PastPaper> _pastPapers = pastPapers;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITenantProvider _tenant = tenant;
        private readonly IHttpContextAccessor _http = http;

        public async Task<DashboardViewModel> BuildDashboardAsync()
        {
            bool isSuperAdmin = _tenant.IsSuperAdmin;
            int? instituteId = _tenant.InstituteId;

            DashboardViewModel model = new();

            DateTime monthStart = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime nextMonthStart = monthStart.AddMonths(1);

            IQueryable<Test> tests = _tests.Query();

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

            Dictionary<QuestionType, int> questionsByType = await _questions.Query()
                                                                            .Where(q => q.IsActive)
                                                                            .GroupBy(q => q.Type)
                                                                            .Select(g => new { Type = g.Key, Count = g.Count() })
                                                                            .ToDictionaryAsync(x => x.Type, x => x.Count);

            model.TotalMcqs = questionsByType.GetValueOrDefault(QuestionType.Mcq);
            model.TotalShortQuestions = questionsByType.GetValueOrDefault(QuestionType.Short);
            model.TotalLongQuestions = questionsByType.GetValueOrDefault(QuestionType.Long);

            var studentStats = await _students.Query().Where(s => s.IsActive)
                                              .GroupBy(_ => 1)
                                              .Select(g => new
                                              {
                                                  Total = g.Count(),
                                                  ThisMonth = g.Count(s => s.CreatedAt >= monthStart && s.CreatedAt < nextMonthStart)
                                              })
                                              .FirstOrDefaultAsync();

            model.TotalStudents = studentStats?.Total ?? 0;
            model.StudentsThisMonth = studentStats?.ThisMonth ?? 0;

            IQueryable<Teacher> teachers = _teachers.Query().Where(t => t.IsActive);

            if (!isSuperAdmin)
            {
                teachers = teachers.Where(t => t.User.InstituteId == instituteId);
            }

            model.TotalTeachers = await teachers.CountAsync();

            var pastPapersByBoard = await _pastPapers.Query()
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

                var instituteStats = await _institutes.Query()
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

                var monthlyNew = await _institutes.Query()
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

            string? layoutUserId = _userManager.GetUserId(_http.HttpContext!.User);

            model.LayoutJson = instituteId is int layoutInstituteId
                ? await _institutes.Query()
                                   .AsNoTracking()
                                   .Where(i => i.Id == layoutInstituteId)
                                   .Select(i => i.DashboardLayout)
                                   .FirstOrDefaultAsync()
                : await _userManager.Users
                                    .AsNoTracking()
                                    .Where(u => u.Id == layoutUserId)
                                    .Select(u => u.DashboardLayout)
                                    .FirstOrDefaultAsync();

            return model;
        }

        public async Task<bool> SaveLayoutAsync(string? layout)
        {
            int? instituteId = _tenant.InstituteId;

            if (instituteId is int iid)
            {
                int rows = await _institutes.Query()
                                            .Where(i => i.Id == iid)
                                            .ExecuteUpdateAsync(s => s.SetProperty(i => i.DashboardLayout, layout)
                                                                      .SetProperty(i => i.UpdatedAt, DateTime.UtcNow));

                return rows > 0;
            }

            string? userId = _userManager.GetUserId(_http.HttpContext!.User);

            if (userId is null)
            {
                return false;
            }

            await _userManager.Users.Where(u => u.Id == userId)
                                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.DashboardLayout, layout));

            return true;
        }
    }
}
