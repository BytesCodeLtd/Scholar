using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scholar.Constants;
using Scholar.Enums;

namespace Scholar.Models.ViewModels
{
    public class CreateStudentViewModel
    {
        [Required(ErrorMessage = Message.FullNameRequired)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Roll Number")]
        public string? RollNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = Message.ClassRequired)]
        [Display(Name = "Class")]
        public int GradeId { get; set; }

        [Display(Name = "Section")]
        public string? Section { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Guardian Name")]
        public string? GuardianName { get; set; }

        [Phone(ErrorMessage = Message.PhoneInvalid)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        /// <summary>Only used/shown for a SuperAdmin, who must choose the institute.</summary>
        [Display(Name = "Institute")]
        public int? InstituteId { get; set; }

        public bool ShowInstitute { get; set; }

        public IEnumerable<SelectListItem> GradeOptions { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> InstituteOptions { get; set; } = new List<SelectListItem>();
    }
}
