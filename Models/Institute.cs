using System.ComponentModel.DataAnnotations;

namespace Scholar.Models
{
    public class Institute : IAuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Institute Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Logo")]
        public string? LogoUrl { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        public string? DashboardLayout { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    }
}
