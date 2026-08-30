using System.ComponentModel.DataAnnotations;
using Scholar.Constants;

namespace Scholar.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = Message.EmailRequired)]
        [EmailAddress(ErrorMessage = Message.EmailInvalid)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = Message.PasswordRequired)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
