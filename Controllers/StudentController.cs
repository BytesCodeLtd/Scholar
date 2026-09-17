using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class StudentController : Controller
    {
        private readonly IStudentService _students;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService students, ILogger<StudentController> logger)
        {
            _students = students;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters? tableParams = null)
        {
            PagedResult<StudentRow> paged = await _students.GetRosterAsync(tableParams ?? new PageParameters());
            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            StudentDetailsViewModel? model = await _students.GetDetailsAsync(id);

            if (model is null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!_students.CanOpenAdmissionForm)
            {
                TempData["Error"] = Message.AccountNotLinkedToInstitute;
                return RedirectToAction(nameof(Index));
            }

            CreateStudentViewModel model = new();
            await _students.PopulateCreateOptionsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel model)
        {
            if (ModelState.IsValid && await _students.CreateAsync(model))
            {
                _logger.LogInformation("Student {Name} created.", model.FullName);
                TempData["Success"] = MsgKey.Success.Created(Key.Student);
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                _logger.LogWarning("Student create failed: no institute resolved.");
                ModelState.AddModelError(nameof(model.InstituteId), "Select an institute.");
            }

            await _students.PopulateCreateOptionsAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (!_students.CanOpenAdmissionForm)
            {
                TempData["Error"] = Message.AccountNotLinkedToInstitute;
                return RedirectToAction(nameof(Index));
            }

            RegisterStudentViewModel model = new();
            await _students.PopulateRegisterOptionsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterStudentViewModel model)
        {
            if (ModelState.IsValid && await _students.RegisterAsync(model))
            {
                _logger.LogInformation("Student {First} {Last} registered.", model.FirstName, model.LastName);
                TempData["Success"] = MsgKey.Success.Created(Key.Student);
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                _logger.LogWarning("Student registration failed: no institute resolved.");
                ModelState.AddModelError(nameof(model.InstituteId), "Select an institute.");
            }

            await _students.PopulateRegisterOptionsAsync(model);
            return View(model);
        }

        [HttpGet]
        public IActionResult Promote()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool ok = await _students.DeactivateAsync(id);
            if (ok)
            {
                _logger.LogInformation("Student {Id} deactivated.", id);
            }
            else
            {
                _logger.LogWarning("Deactivate failed: student {Id} not found.", id);
            }

            TempData[ok ? "Success" : "Error"] = ok ? "Student deactivated." : "Student not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            bool ok = await _students.ActivateAsync(id);
            if (ok)
            {
                _logger.LogInformation("Student {Id} activated.", id);
            }
            else
            {
                _logger.LogWarning("Activate failed: student {Id} not found.", id);
            }

            TempData[ok ? "Success" : "Error"] = ok ? "Student activated." : "Student not found.";
            return RedirectToAction(nameof(Index));
        }
    }
}
