using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Scholar.Enums;

namespace Scholar.Models
{
    /// <summary>A billable fee head (e.g. Tuition, Admission, Transport) owned by an institute.</summary>
    public class FeeCategory : IAuditableEntity, ITenantEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        // Suggested amount pre-filled when generating invoices; editable per invoice.
        [Display(Name = "Default Amount")]
        public decimal DefaultAmount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>A fee bill (challan) issued to a student for a billing period.</summary>
    public class Invoice : IAuditableEntity, ITenantEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public int StudentId { get; set; }

        public Student Student { get; set; } = null!;

        // Human-readable billing period, e.g. "September 2026" or "Term 1 2026".
        [Display(Name = "Period")]
        public string Period { get; set; } = string.Empty;

        [Display(Name = "Due Date")]
        public DateOnly DueDate { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // --- Derived (not stored) ---
        [NotMapped]
        public decimal Total => Items?.Sum(i => i.Amount) ?? 0m;

        [NotMapped]
        public decimal Paid => Payments?.Sum(p => p.Amount) ?? 0m;

        [NotMapped]
        public decimal Balance => Total - Paid;

        [NotMapped]
        public bool IsOverdue => Status != InvoiceStatus.Paid && DueDate < DateOnly.FromDateTime(DateTime.Today);
    }

    /// <summary>A single line on an invoice, tied to a fee category.</summary>
    public class InvoiceItem : IAuditableEntity
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; } = null!;

        // Nullable so history survives if the category is later removed.
        public int? FeeCategoryId { get; set; }

        public FeeCategory? FeeCategory { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>A payment recorded against an invoice (supports partial payments).</summary>
    public class Payment : IAuditableEntity
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; } = null!;

        public decimal Amount { get; set; }

        [Display(Name = "Paid On")]
        public DateOnly PaidOn { get; set; }

        [Display(Name = "Method")]
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

        [Display(Name = "Note")]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
