using System.ComponentModel.DataAnnotations;
using Scholar.Constants;

namespace Scholar.Models.ViewModels
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = Message.FullNameRequired)]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = Message.EmailRequired)]
        [EmailAddress(ErrorMessage = Message.EmailInvalid)]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
