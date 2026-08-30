using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholar.Models;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        private readonly IRepository<Grade> _gradeRepository;

        public GradeController(IRepository<Grade> gradeRepository)
        {
            _gradeRepository = gradeRepository;
        }

        public async Task<IActionResult> Index(int boardId)
        {
            List<Grade> grades = await _gradeRepository.Query()
                                                       .Where(g => g.Subjects.Any(s => s.BoardId == boardId))
                                                       .OrderBy(g => g.Id)
                                                       .ToListAsync();
            ViewBag.BoardId = boardId;
            return View(grades);
        }

    }
}
