using System.ComponentModel.DataAnnotations;
using Scholar.Constants;

namespace Scholar.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = Message.FullNameRequired)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = Message.EmailRequired)]
        [EmailAddress(ErrorMessage = Message.EmailInvalid)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = Message.InstituteRequired)]
        [Display(Name = "Institute Name")]
        public string InstituteName { get; set; } = string.Empty;

        [Display(Name = "Institute Logo")]
        public IFormFile? Logo { get; set; }

        [Required(ErrorMessage = Message.PasswordRequired)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = Message.RoleRequired)]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;
    }
}
