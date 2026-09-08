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
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Teacher> _teacherRepository;

        public DashboardController(
            IRepository<Test> test,
            IRepository<Institute> institute,
            IRepository<Question> question,
            IRepository<Student> student,
            IRepository<Teacher> teacher)
        {
            _testRepository = test;
            _instituteRepository = institute;
            _questionRepository = question;
            _studentRepository = student;
            _teacherRepository = teacher;
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

            IQueryable<Student> students = _studentRepository.Query().Where(s => s.IsActive);

            if (!isSuperAdmin)
            {
                students = students.Where(s => s.InstituteId == instituteId);
            }

            model.TotalStudents = await students.CountAsync();
            model.StudentsThisMonth = await students.CountAsync(s => s.CreatedAt >= monthStart && s.CreatedAt < nextMonthStart);

            IQueryable<Teacher> teachers = _teacherRepository.Query().Where(t => t.IsActive);

            if (!isSuperAdmin)
            {
                teachers = teachers.Where(t => t.User.InstituteId == instituteId);
            }

            model.TotalTeachers = await teachers.CountAsync();

            if (isSuperAdmin)
            {
                model.IsSuperAdmin = true;
                model.TotalInstitutes = await _instituteRepository.Query().CountAsync();
                model.ActiveInstitutes = await _instituteRepository.Query().CountAsync(i => i.IsActive);
                model.InstitutesThisMonth = await _instituteRepository.Query()
                                                                      .CountAsync(i => i.CreatedAt >= monthStart && i.CreatedAt < nextMonthStart);

                // Cumulative institute growth over the last 6 months for the area chart.
                const int months = 6;
                DateTime firstMonth = monthStart.AddMonths(-(months - 1));

                int baseline = await _instituteRepository.Query().CountAsync(i => i.CreatedAt < firstMonth);

                var monthlyNew = await _instituteRepository.Query()
                                                           .Where(i => i.CreatedAt >= firstMonth && i.CreatedAt < nextMonthStart)
                                                           .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                                                           .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                                                           .ToListAsync();

                int running = baseline;
                for (int i = 0; i < months; i++)
                {
                    DateTime m = firstMonth.AddMonths(i);
                    running += monthlyNew.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Count ?? 0;
                    model.InstituteChartLabels.Add(m.ToString("MMM"));
                    model.InstituteChartData.Add(running);
                }
            }

            return View(model);
        }
    }
}
