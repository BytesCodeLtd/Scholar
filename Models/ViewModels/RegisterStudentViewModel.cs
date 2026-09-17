using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Constants;
using Scholar.Enums;

namespace Scholar.Models.ViewModels
{
    /// <summary>Backs the full student admission (registration) form.</summary>
    public class RegisterStudentViewModel
    {
        // --- Student Admission ----------------------------------------------
        [Required(ErrorMessage = "Admission number is required.")]
        [Display(Name = "Admission Number")]
        public string AdmissionNumber { get; set; } = string.Empty;

        [Display(Name = "Roll Number")]
        public string? RollNumber { get; set; }

        [Display(Name = "Admission Session")]
        public string? AdmissionSession { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = Message.ClassRequired)]
        [Display(Name = "Class")]
        public int GradeId { get; set; }

        [Display(Name = "Section")]
        public string? Section { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Select a gender.")]
        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Category")]
        public string? Category { get; set; }

        [Display(Name = "Religion")]
        public string? Religion { get; set; } = "Islam";

        [Required(ErrorMessage = Message.PhoneInvalid)]
        [Phone(ErrorMessage = Message.PhoneInvalid)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Admission date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Admission Date")]
        public DateTime? AdmissionDate { get; set; } = DateTime.Today;

        [Display(Name = "Student Photo")]
        public IFormFile? StudentPhoto { get; set; }

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

        /// <summary>Maps to the student's institute; labelled "Academy" on the form.</summary>
        [Display(Name = "Academy")]
        public int? InstituteId { get; set; }

        [DataType(DataType.Date)]
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

        // --- Parent / Guardian Detail ---------------------------------------
        [Display(Name = "Father Name")]
        public string? FatherName { get; set; }

        [Display(Name = "Father CNIC")]
        public string? FatherCnic { get; set; }

        [Display(Name = "Father Phone")]
        public string? FatherPhone { get; set; }

        [Display(Name = "Father Occupation")]
        public string? FatherOccupation { get; set; }

        [Display(Name = "Father Photo")]
        public IFormFile? FatherPhoto { get; set; }

        [Display(Name = "Mother Name")]
        public string? MotherName { get; set; }

        [Display(Name = "Mother CNIC")]
        public string? MotherCnic { get; set; }

        [Display(Name = "Mother Phone")]
        public string? MotherPhone { get; set; }

        [Display(Name = "Mother Occupation")]
        public string? MotherOccupation { get; set; }

        [Display(Name = "Mother Photo")]
        public IFormFile? MotherPhoto { get; set; }

        [Required(ErrorMessage = "Select who the guardian is.")]
        [Display(Name = "Guardian Is")]
        public GuardianType GuardianType { get; set; } = Enums.GuardianType.Father;

        [Required(ErrorMessage = "Guardian name is required.")]
        [Display(Name = "Guardian Name")]
        public string GuardianName { get; set; } = string.Empty;

        [Display(Name = "Guardian Relation")]
        public string? GuardianRelation { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Guardian Email")]
        public string? GuardianEmail { get; set; }

        [Display(Name = "Guardian Photo")]
        public IFormFile? GuardianPhoto { get; set; }

        [Required(ErrorMessage = "Guardian phone is required.")]
        [Phone(ErrorMessage = Message.PhoneInvalid)]
        [Display(Name = "Guardian Phone")]
        public string GuardianPhone { get; set; } = string.Empty;

        [Display(Name = "Guardian Occupation")]
        public string? GuardianOccupation { get; set; }

        [Display(Name = "Guardian Address")]
        public string? GuardianAddress { get; set; }

        // --- Fee Details -----------------------------------------------------
        [Display(Name = "Concession Type")]
        public string? ConcessionType { get; set; }

        [DataType(DataType.Date)]
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

        // --- Form context ----------------------------------------------------
        public bool ShowInstitute { get; set; }

        public string? LastAdmissionNumber { get; set; }

        public string? LastRollNumber { get; set; }

        public IReadOnlyList<StudentClassOption> ClassOptions { get; set; } = [];

        public IEnumerable<SelectListItem> InstituteOptions { get; set; } = new List<SelectListItem>();
    }

    public class StudentClassOption
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<string> Sections { get; set; } = [];
    }
}
