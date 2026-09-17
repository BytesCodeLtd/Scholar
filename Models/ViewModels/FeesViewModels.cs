using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Enums;

namespace Scholar.Models.ViewModels
{
    // ---- DataTable row DTOs ----

    public class FeeCategoryRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DefaultAmount { get; set; } = string.Empty;
        public string Institute { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class InvoiceRow
    {
        public int Id { get; set; }
        public string Student { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
        public string Balance { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Institute { get; set; } = string.Empty;
    }

    // ---- Forms ----

    public class FeeCategoryFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 100_000_000)]
        [Display(Name = "Default Amount (Rs.)")]
        public decimal DefaultAmount { get; set; }

        // Super admin picks the institute; institute admin is fixed to their own.
        public bool ShowInstitute { get; set; }
        public int? InstituteId { get; set; }
        public IEnumerable<SelectListItem> InstituteOptions { get; set; } = new List<SelectListItem>();
    }

    public class GenerateLineViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Include { get; set; }
        public decimal Amount { get; set; }
    }

    public class GenerateInvoicesViewModel
    {
        public bool ShowInstitute { get; set; }
        public int? InstituteId { get; set; }
        public IEnumerable<SelectListItem> InstituteOptions { get; set; } = new List<SelectListItem>();

        [Required(ErrorMessage = "Please select a class.")]
        [Display(Name = "Class")]
        public int? GradeId { get; set; }
        public IEnumerable<SelectListItem> GradeOptions { get; set; } = new List<SelectListItem>();

        [Display(Name = "Section (optional)")]
        public string? Section { get; set; }

        [Required(ErrorMessage = "Enter a billing period.")]
        [Display(Name = "Billing Period")]
        public string Period { get; set; } = string.Empty;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(10));

        public List<GenerateLineViewModel> Lines { get; set; } = new();
    }

    // ---- Invoice detail ----

    public class PaymentLine
    {
        public decimal Amount { get; set; }
        public DateOnly PaidOn { get; set; }
        public PaymentMethod Method { get; set; }
        public string? Note { get; set; }
    }

    public class InvoiceItemLine
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class InvoiceDetailsViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? RollNumber { get; set; }
        public string Class { get; set; } = string.Empty;
        public string? Section { get; set; }
        public string Institute { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public bool IsOverdue { get; set; }

        public List<InvoiceItemLine> Items { get; set; } = new();
        public List<PaymentLine> Payments { get; set; } = new();

        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance => Total - Paid;

        // For the "record payment" form.
        public decimal PaymentAmount { get; set; }
        public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? PaymentNote { get; set; }
    }
}
