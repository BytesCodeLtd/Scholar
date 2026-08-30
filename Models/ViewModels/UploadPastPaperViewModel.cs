using System.ComponentModel.DataAnnotations;

namespace Scholar.Models.ViewModels
{
    public class UploadPastPaperViewModel
    {
        [Required(ErrorMessage = "Select a subject.")]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Range(1900, 2100, ErrorMessage = "Enter a valid year.")]
        [Display(Name = "Year")]
        public int? Year { get; set; }

        [Display(Name = "PDF File")]
        public IFormFile? File { get; set; }
    }
}
