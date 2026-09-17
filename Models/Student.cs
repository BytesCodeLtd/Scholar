using System.ComponentModel.DataAnnotations;
using Scholar.Enums;

namespace Scholar.Models
{
    public class Student : IAuditableEntity, ITenantEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public int? ClassId { get; set; }

        public InstituteClass? Class { get; set; }

        // --- Admission -------------------------------------------------------
        [Display(Name = "Admission Number")]
        public string? AdmissionNumber { get; set; }

        [Display(Name = "Admission Session")]
        public string? AdmissionSession { get; set; }

        [Display(Name = "Admission Date")]
        public DateTime? AdmissionDate { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Roll Number")]
        public string? RollNumber { get; set; }

        [Display(Name = "Section")]
        public string? Section { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Category")]
        public string? Category { get; set; }

        [Display(Name = "Religion")]
        public string? Religion { get; set; }

        [Phone]
        [Display(Name = "Mobile Number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Student Photo")]
        public string? PhotoUrl { get; set; }

        [Display(Name = "Caste")]
        public string? Caste { get; set; }

        [Display(Name = "Blood Group")]
        public string? BloodGroup { get; set; }

        [Display(Name = "Height")]
        public string? Height { get; set; }

        [Display(Name = "Weight")]
        public string? Weight { get; set; }

        [Display(Name = "Family No")]
        public string? FamilyNo { get; set; }

        [Display(Name = "Is Family Head")]
        public bool? IsFamilyHead { get; set; }

        [Display(Name = "As On Date")]
        public DateTime? AsOnDate { get; set; }

        [Display(Name = "B Form/CNIC #")]
        public string? BFormCnic { get; set; }

        [Display(Name = "Whatsapp #")]
        public string? WhatsappNumber { get; set; }

        [Display(Name = "Card Id")]
        public string? CardId { get; set; }

        [Display(Name = "Ref By")]
        public string? RefBy { get; set; }

        [Display(Name = "Last Class Attended")]
        public string? LastClassAttended { get; set; }

        [Display(Name = "Previous School Details")]
        public string? PreviousSchoolDetails { get; set; }

        // --- Parent / Guardian ----------------------------------------------
        [Display(Name = "Father Name")]
        public string? FatherName { get; set; }

        [Display(Name = "Father CNIC")]
        public string? FatherCnic { get; set; }

        [Display(Name = "Father Phone")]
        public string? FatherPhone { get; set; }

        [Display(Name = "Father Occupation")]
        public string? FatherOccupation { get; set; }

        [Display(Name = "Father Photo")]
        public string? FatherPhotoUrl { get; set; }

        [Display(Name = "Mother Name")]
        public string? MotherName { get; set; }

        [Display(Name = "Mother CNIC")]
        public string? MotherCnic { get; set; }

        [Display(Name = "Mother Phone")]
        public string? MotherPhone { get; set; }

        [Display(Name = "Mother Occupation")]
        public string? MotherOccupation { get; set; }

        [Display(Name = "Mother Photo")]
        public string? MotherPhotoUrl { get; set; }

        [Display(Name = "Guardian Is")]
        public GuardianType? GuardianType { get; set; }

        [Display(Name = "Guardian Name")]
        public string? GuardianName { get; set; }

        [Display(Name = "Guardian Relation")]
        public string? GuardianRelation { get; set; }

        [Display(Name = "Guardian Email")]
        public string? GuardianEmail { get; set; }

        [Display(Name = "Guardian Phone")]
        public string? GuardianPhone { get; set; }

        [Display(Name = "Guardian Occupation")]
        public string? GuardianOccupation { get; set; }

        [Display(Name = "Guardian Address")]
        public string? GuardianAddress { get; set; }

        [Display(Name = "Guardian Photo")]
        public string? GuardianPhotoUrl { get; set; }

        // --- Fee details -----------------------------------------------------
        [Display(Name = "Concession Type")]
        public string? ConcessionType { get; set; }

        [Display(Name = "Concession Valid Till")]
        public DateTime? ConcessionValidTill { get; set; }

        [Display(Name = "Tuition Fee Head")]
        public string? TuitionFeeHead { get; set; }

        [Display(Name = "Admission Head")]
        public string? AdmissionHead { get; set; }

        [Display(Name = "Admission Discount")]
        public decimal? AdmissionDiscount { get; set; }

        [Display(Name = "Terms And Conditions")]
        public string? TermsAndConditions { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
