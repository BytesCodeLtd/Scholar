using System.ComponentModel.DataAnnotations;
using Scholar.Enums;

namespace Scholar.Models
{
    /// <summary>
    /// A student enrolled at an institute. A plain roster record (no login account);
    /// belongs to one institute and is placed in a grade/class.
    /// </summary>
    public class Student : IAuditableEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        /// <summary>The class the student is in (curriculum grade).</summary>
        public int GradeId { get; set; }

        public Grade Grade { get; set; } = null!;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Roll Number")]
        public string? RollNumber { get; set; }

        [Display(Name = "Section")]
        public string? Section { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Guardian Name")]
        public string? GuardianName { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
