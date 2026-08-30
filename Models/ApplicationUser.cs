using Microsoft.AspNetCore.Identity;

namespace Scholar.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public int? InstituteId { get; set; }

        public Institute? Institute { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
