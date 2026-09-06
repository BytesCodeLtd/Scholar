using Microsoft.AspNetCore.Authorization;
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
        private readonly IRepository<Test> _testRepository;
        private readonly IRepository<Institute> _instituteRepository;
        private readonly IRepository<Question> _questionRepository;

        public DashboardController(
            IRepository<Test> test,
            IRepository<Institute> institute,
            IRepository<Question> question)
        {
            _testRepository = test;
            _instituteRepository = institute;
            _questionRepository = question;
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

            model.TotalTests = await tests.CountAsync();
            model.SavedTests = await tests.CountAsync(t => t.IsActive);
            model.TestsThisMonth = await tests.CountAsync(t => t.CreatedAt >= monthStart && t.CreatedAt < nextMonthStart);

            Dictionary<QuestionType, int> questionsByType = await _questionRepository.Query()
                                                                                     .Where(q => q.IsActive)
                                                                                     .GroupBy(q => q.Type)
                                                                                     .Select(g => new { Type = g.Key, Count = g.Count() })
                                                                                     .ToDictionaryAsync(x => x.Type, x => x.Count);

            model.TotalMcqs = questionsByType.GetValueOrDefault(QuestionType.Mcq);
            model.TotalShortQuestions = questionsByType.GetValueOrDefault(QuestionType.Short);
            model.TotalLongQuestions = questionsByType.GetValueOrDefault(QuestionType.Long);

            if (isSuperAdmin)
            {
                model.IsSuperAdmin = true;
                model.TotalInstitutes = await _instituteRepository.Query().CountAsync();
                model.ActiveInstitutes = await _instituteRepository.Query().CountAsync(i => i.IsActive);
                model.InstitutesThisMonth = await _instituteRepository.Query()
                                                                      .CountAsync(i => i.CreatedAt >= monthStart && i.CreatedAt < nextMonthStart);
            }

            return View(model);
        }
    }
}
