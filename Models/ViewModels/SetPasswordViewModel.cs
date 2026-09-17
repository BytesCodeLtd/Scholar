using System.ComponentModel.DataAnnotations;

namespace Scholar.Models.ViewModels
{
    public class SetPasswordViewModel
    {
        public string UserId { get; set; } = string.Empty;

        // Base64Url-encoded password-reset token from the emailed link.
        public string Token { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
