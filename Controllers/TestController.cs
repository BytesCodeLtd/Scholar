using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Identity;
using Scholar.Common.Papers;
using Scholar.Enums;
using Scholar.Models;
using Scholar.Models.ViewModels;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly IRepository<Test> _tests;
        private readonly IRepository<Question> _questions;
        private readonly IRepository<Chapter> _chapters;
        private readonly IRepository<Subject> _subjects;
        private readonly IRepository<TestSettings> _testSettings;
        private readonly IRepository<Institute> _institutes;

        public TestController(
            IRepository<Test> tests,
            IRepository<Question> questions,
            IRepository<Chapter> chapters,
            IRepository<Subject> subjects,
            IRepository<TestSettings> testSettings,
            IRepository<Institute> institutes)
        {
            _tests = tests;
            _questions = questions;
            _chapters = chapters;
            _subjects = subjects;
            _testSettings = testSettings;
            _institutes = institutes;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            bool isSuperAdmin = User.IsInRole(Constants.Roles.SuperAdmin);
            int? instituteId = User.GetInstituteId();

            ViewBag.IsSuperAdmin = isSuperAdmin;

            List<PaperCard> papers = [];

            if (isSuperAdmin || instituteId is not null)
            {
                IQueryable<Test> query = _tests.Query().Where(t => t.IsActive);

                if (!isSuperAdmin)
                {
                    query = query.Where(t => t.InstituteId == instituteId);
                }

                papers = await query
                                     .OrderByDescending(t => t.CreatedAt)
                                     .Select(t => new PaperCard
                                     {
                                         Id = t.Id,
                                         Title = t.Title,
                                         Type = t.Type,
                                         Date = t.Date,
                                         Subject = t.Subject.Name,
                                         Institute = t.Institute.Name,
                                         TotalMarks = t.TotalMarks,
                                         DurationMinutes = t.DurationMinutes,
                                         QuestionCount = t.Questions.Count(q => q.IsActive),
                                         CreatedAt = t.CreatedAt
                                     })
                                     .ToListAsync();
            }

            return View(papers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int[] selectedTopicIds, int subjectId, int? instituteId)
        {
            Branding? branding = await LoadBrandingAsync(instituteId);

            if (branding is null)
            {
                TempData["Error"] = User.IsInRole(Constants.Roles.SuperAdmin)
                    ? "Select an institute to generate a paper for."
                    : "Your account isn't linked to an institute, so papers can't be branded.";
                return RedirectToAction("Index", "Dashboard");
            }

            Subject? subject = await _subjects.Query()
                                              .AsNoTracking()
                                              .Include(s => s.Grade)
                                              .FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject is null)
            {
                return NotFound();
            }

            List<Chapter> chapters = await _chapters.Query().AsNoTracking()
                                                    .Where(c => c.SubjectId == subjectId)
                                                    .Include(c => c.Topics)
                                                    .OrderBy(c => c.Number)
                                                    .ToListAsync();

            PaperBuilderViewModel vm = new()
            {
                SubjectId = subjectId,
                GradeId = subject.GradeId,
                SubjectName = subject.Name,
                GradeName = subject.Grade.Name,
                InstituteId = branding.Institute.Id,
                Settings = branding.Settings,
                Chapters = chapters,
                PreselectedTopicIds = [.. (selectedTopicIds ?? [])]
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> SearchQuestions(int[] topicIds, QuestionType type, QuestionCategory[] categories)
        {
            if (topicIds is null || topicIds.Length == 0)
            {
                return PartialView("_QuestionPicker", new List<Question>());
            }

            IQueryable<Question> query = _questions.Query()
                                                   .AsNoTracking()
                                                   .Include(q => q.Options)
                                                   .Where(q => q.IsActive && topicIds.Contains(q.TopicId) && q.Type == type);

            if (categories is { Length: > 0 })
            {
                query = query.Where(q => categories.Contains(q.Category));
            }

            List<Question> questions = await query.OrderBy(q => q.Id)
                                                  .ToListAsync();

            return PartialView("_QuestionPicker", questions);
        }

        [HttpPost]
        public async Task<IActionResult> RenderSections([FromBody] RenderSectionsRequest request)
        {
            Branding? branding = await LoadBrandingAsync(request?.InstituteId);

            if (branding is null)
            {
                return Forbid();
            }

            PaperSectionsViewModel vm = new()
            {
                Settings = branding.Settings,
                Sections = await BuildSectionModelsAsync(request?.Sections ?? new())
            };

            return PartialView("_PaperSectionsList", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SavePaperRequest request)
        {
            Branding? branding = await LoadBrandingAsync(request?.InstituteId);

            if (branding is null)
            {
                return Forbid();
            }

            if (request is null || string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "Give the paper a title before saving." });
            }

            List<PaperSectionInput> inputs = [.. (request.Sections ?? []).Where(s => s.QuestionIds is { Count: > 0 })];

            if (inputs.Count == 0)
            {
                return BadRequest(new { message = "Add at least one question section before saving." });
            }

            List<PaperSectionRenderModel> models = await BuildSectionModelsAsync(inputs);

            int totalMarks = models.Sum(m => m.TotalMarks);

            Test test = new()
            {
                Title = request.Title.Trim(),
                Type = request.PaperType?.Trim() ?? string.Empty,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                InstituteId = branding.Institute.Id,
                SubjectId = request.SubjectId,
                TotalMarks = totalMarks,
                DurationMinutes = request.DurationMinutes,
                PaperSettingsSnapshot = JsonSerializer.Serialize(branding.Settings)
            };

            int order = 1;
            foreach ((PaperSectionInput input, PaperSectionRenderModel model) in inputs.Zip(models))
            {
                TestSection section = new()
                {
                    Test = test,
                    Order = order++,
                    Type = input.Type,
                    Instruction = SectionInstructions.For(input.Type),
                    MarksPerQuestion = input.MarksPerQuestion
                };

                int qOrder = 1;
                foreach (Question q in model.Questions)
                {
                    section.Questions.Add(new TestQuestion
                    {
                        Test = test,
                        QuestionId = q.Id,
                        Order = qOrder++
                    });
                }

                test.Sections.Add(section);
            }

            await _tests.AddAsync(test);
            await _tests.SaveChangesAsync();

            return Ok(new { id = test.Id, redirectUrl = Url.Action(nameof(Print), new { id = test.Id }) });
        }

        [HttpGet]
        public async Task<IActionResult> Print(int id)
        {
            Test? test = await _tests.Query()
                                     .AsNoTracking()
                                     .Include(t => t.Subject)
                                         .ThenInclude(s => s.Grade)
                                     .Include(t => t.Sections)
                                         .ThenInclude(s => s.Questions)
                                             .ThenInclude(tq => tq.Question)
                                                 .ThenInclude(q => q.Options)
                                     .FirstOrDefaultAsync(t => t.Id == id);

            if (test is null)
            {
                return NotFound();
            }

            // Scope: institute members only (super admins may view any).
            if (!User.IsInRole(Constants.Roles.SuperAdmin) && test.InstituteId != User.GetInstituteId())
            {
                return Forbid();
            }

            PaperRenderSettings settings = ResolveSnapshot(test);

            List<PaperSectionRenderModel> sections = [];
            int start = 1;

            foreach (TestSection s in test.Sections.OrderBy(s => s.Order))
            {
                List<Question> qs = [.. s.Questions.OrderBy(q => q.Order).Select(q => q.Question)];
                sections.Add(new PaperSectionRenderModel
                {
                    SectionNumber = s.Order,
                    Instruction = s.Instruction,
                    Type = s.Type,
                    MarksPerQuestion = s.MarksPerQuestion,
                    StartIndex = start,
                    Questions = qs
                });
                start += qs.Count;
            }

            PaperDocumentViewModel vm = new()
            {
                TestId = test.Id,
                Title = test.Title,
                PaperType = test.Type,
                DurationMinutes = test.DurationMinutes,
                TotalMarks = test.TotalMarks,
                SubjectName = test.Subject.Name,
                GradeName = test.Subject.Grade.Name,
                Settings = settings,
                Sections = sections
            };

            return View(vm);
        }

        private sealed record Branding(Institute Institute, PaperRenderSettings Settings);

        private async Task<Branding?> LoadBrandingAsync(int? requestedInstituteId = null)
        {
            int? instituteId = ResolveInstituteId(requestedInstituteId);

            if (instituteId is null)
            {
                return null;
            }

            Institute? institute = await _institutes.GetByIdAsync(instituteId.Value);

            if (institute is null)
            {
                return null;
            }

            TestSettings settings = await _testSettings.Query()
                                                       .AsNoTracking()
                                                       .FirstOrDefaultAsync(p => p.InstituteId == institute.Id)
                                     ?? new TestSettings { InstituteId = institute.Id };

            return new Branding(institute, PaperRenderSettings.From(settings, institute));
        }

        private int? ResolveInstituteId(int? requestedInstituteId)
            => User.IsInRole(Constants.Roles.SuperAdmin)
                ? requestedInstituteId
                : User.GetInstituteId();

        private async Task<List<PaperSectionRenderModel>> BuildSectionModelsAsync(List<PaperSectionInput> inputs)
        {
            List<int> allIds = inputs.SelectMany(s => s.QuestionIds).Distinct().ToList();

            Dictionary<int, Question> byId = await _questions.Query()
                                                             .AsNoTracking()
                                                             .Include(q => q.Options)
                                                             .Include(q => q.Topic)
                                                                .ThenInclude(t => t.Chapter)
                                                             .Where(q => allIds.Contains(q.Id))
                                                             .ToDictionaryAsync(q => q.Id);

            List<PaperSectionRenderModel> models = [];
            int start = 1;
            int order = 1;

            foreach (PaperSectionInput input in inputs)
            {
                List<Question> questions = [.. input.QuestionIds.Where(byId.ContainsKey).Select(id => byId[id])];

                models.Add(new PaperSectionRenderModel
                {
                    SectionNumber = order++,
                    Instruction = SectionInstructions.For(input.Type),
                    Type = input.Type,
                    MarksPerQuestion = input.MarksPerQuestion,
                    StartIndex = start,
                    Questions = questions
                });

                start += questions.Count;
            }

            return models;
        }


        private static PaperRenderSettings ResolveSnapshot(Test test)
        {
            if (!string.IsNullOrWhiteSpace(test.PaperSettingsSnapshot))
            {
                try
                {
                    PaperRenderSettings? snap = JsonSerializer.Deserialize<PaperRenderSettings>(test.PaperSettingsSnapshot);

                    if (snap is not null)
                    {
                        return snap;
                    }
                }
                catch (JsonException)
                {
                    // Fall through to defaults on a corrupt snapshot.
                }
            }

            return new PaperRenderSettings { InstituteName = test.Institute?.Name ?? string.Empty };
        }
    }
}
