using System.ComponentModel.DataAnnotations;
using Scholar.Constants;

namespace Scholar.Models.ViewModels
{
    public class CreateUserViewModel
    {
        public int? InstituteId { get; set; }

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

        [Display(Name = "Institute Address")]
        public string? InstituteAddress { get; set; }

        [Display(Name = "Institute Logo")]
        public IFormFile? Logo { get; set; }

        // Existing logo shown when editing; a new upload replaces it.
        public string? LogoUrl { get; set; }

        // Required only when creating (enforced in the controller); on update, a blank
        // value means "keep the current password".
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required(ErrorMessage = Message.RoleRequired)]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;
    }
}
