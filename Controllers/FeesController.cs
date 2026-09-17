using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Paging;
using Scholar.Constants;
using Scholar.Enums;
using Scholar.Models.ViewModels;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.InstituteAdmin)]
    public class FeesController : Controller
    {
        private readonly IFeesService _fees;
        private readonly ILogger<FeesController> _logger;

        public FeesController(IFeesService fees, ILogger<FeesController> logger)
        {
            _fees = fees;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PageParameters? tableParams = null, string? status = null)
        {
            PagedResult<InvoiceRow> paged = await _fees.GetInvoicesAsync(tableParams ?? new PageParameters(), status);

            ViewBag.Status = status;
            ViewBag.IsSuperAdmin = _fees.IsSuperAdmin;

            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            InvoiceDetailsViewModel? model = await _fees.GetInvoiceDetailsAsync(id);

            if (model is null)
            {
                TempData["Error"] = "Invoice not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(int id, decimal amount, DateOnly paidOn, PaymentMethod method, string? note)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Enter a payment amount greater than zero.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (!await _fees.RecordPaymentAsync(id, amount, paidOn, method, note))
            {
                _logger.LogWarning("Record payment failed: invoice {Id} not found.", id);
                TempData["Error"] = "Invoice not found.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Payment of {Amount} recorded on invoice {Id}.", amount, id);
            TempData["Success"] = $"Payment of {Common.Money.Pkr(amount)} recorded.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendReminder(int id)
        {
            bool sent = await _fees.SendReminderAsync(id);
            if (sent)
            {
                _logger.LogInformation("Fee reminder sent for invoice {Id}.", id);
            }
            else
            {
                _logger.LogWarning("Fee reminder not sent for invoice {Id}.", id);
            }

            TempData[sent ? "Success" : "Error"] = sent
                ? "Reminder email sent."
                : "Couldn't send the reminder — check the guardian email and email settings.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // ---- Generate invoices for a class --------------------------------

        [HttpGet]
        public async Task<IActionResult> Generate(int? instituteId)
        {
            GenerateInvoicesViewModel model = await _fees.BuildGenerateFormAsync(instituteId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(GenerateInvoicesViewModel model)
        {
            if (_fees.IsSuperAdmin && model.InstituteId is null)
            {
                ModelState.AddModelError(nameof(model.InstituteId), "Select an institute.");
            }

            if (!model.Lines.Any(l => l.Include && l.Amount > 0))
            {
                ModelState.AddModelError(string.Empty, "Select at least one fee category with an amount.");
            }

            if (ModelState.IsValid)
            {
                int? created = await _fees.GenerateAsync(model);
                if (created is not null)
                {
                    _logger.LogInformation("Generated {Count} invoice(s) for {Period}.", created, model.Period);
                    TempData["Success"] = created > 0
                        ? $"Generated {created} invoice(s) for {model.Period}."
                        : $"No new invoices created — every student already had one for {model.Period}.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("Invoice generation found no active students for grade {GradeId}.", model.GradeId);
                ModelState.AddModelError(string.Empty, "No active students found for that class/section.");
            }

            await _fees.PopulateGenerateOptionsAsync(model);
            return View(model);
        }

        // ---- Fee categories ------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Categories([FromQuery] PageParameters? tableParams = null)
        {
            PagedResult<FeeCategoryRow> paged = await _fees.GetCategoriesAsync(tableParams ?? new PageParameters());

            ViewBag.IsSuperAdmin = _fees.IsSuperAdmin;

            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryForm(int? id)
        {
            FeeCategoryFormViewModel? model = await _fees.BuildCategoryFormAsync(id);

            if (model is null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Categories));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCategory(FeeCategoryFormViewModel model)
        {
            if (_fees.IsSuperAdmin && model.InstituteId is null)
            {
                ModelState.AddModelError(nameof(model.InstituteId), "Select an institute.");
            }

            if (!ModelState.IsValid)
            {
                await _fees.PopulateCategoryOptionsAsync(model);
                return View(nameof(CategoryForm), model);
            }

            if (!await _fees.SaveCategoryAsync(model))
            {
                _logger.LogWarning("Save fee category failed: category {Id} not found.", model.Id);
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Categories));
            }

            _logger.LogInformation("Fee category {Name} {Action}.", model.Name, model.Id is null ? "created" : "updated");
            TempData["Success"] = model.Id is null ? "Fee category created." : "Fee category updated.";
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            bool ok = await _fees.DeleteCategoryAsync(id);
            if (ok)
            {
                _logger.LogInformation("Fee category {Id} deleted.", id);
            }
            else
            {
                _logger.LogWarning("Delete fee category failed: {Id} not found.", id);
            }

            TempData[ok ? "Success" : "Error"] = ok ? "Fee category deleted." : "Category not found.";
            return RedirectToAction(nameof(Categories));
        }
    }
}
