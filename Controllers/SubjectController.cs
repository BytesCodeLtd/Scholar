using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholar.Models;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        private readonly IRepository<Subject> _subjectRepository;

        public SubjectController(IRepository<Subject> subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<IActionResult> Index(int boardId, int gradeId)
        {
            // A subject is scoped by both board and grade (both FKs live on Subject).
            List<Subject> subjects = await _subjectRepository.Query()
                                                             .Where(s => s.BoardId == boardId && s.GradeId == gradeId)
                                                             .OrderBy(s => s.Id)
                                                             .ToListAsync();

            ViewBag.BoardId = boardId;
            ViewBag.GradeId = gradeId;

            return View(subjects);
        }

    }
}
