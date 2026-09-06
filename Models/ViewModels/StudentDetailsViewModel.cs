using Scholar.Enums;

namespace Scholar.Models.ViewModels
{
    /// <summary>Read-only projection of a student for the details page.</summary>
    public class StudentDetailsViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? RollNumber { get; set; }

        public string Class { get; set; } = string.Empty;

        public string? Section { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? GuardianName { get; set; }

        public string? PhoneNumber { get; set; }

        public string Institute { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        /// <summary>Age in whole years from date of birth, if known.</summary>
        public int? Age
        {
            get
            {
                if (DateOfBirth is not DateTime dob)
                {
                    return null;
                }

                DateTime today = DateTime.Today;
                int age = today.Year - dob.Year;
                if (dob.Date > today.AddYears(-age))
                {
                    age--;
                }

                return age < 0 ? null : age;
            }
        }

        /// <summary>Up to two uppercase initials for the avatar.</summary>
        public string Initials
        {
            get
            {
                string[] parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                {
                    return "?";
                }

                return parts.Length == 1
                    ? parts[0][..1].ToUpperInvariant()
                    : (parts[0][..1] + parts[^1][..1]).ToUpperInvariant();
            }
        }
    }
}
