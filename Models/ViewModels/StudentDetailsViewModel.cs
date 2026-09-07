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

        // ---- Attendance summary (all-time) + recent history ----------------
        public int PresentCount { get; set; }

        public int AbsentCount { get; set; }

        public int LateCount { get; set; }

        public int LeaveCount { get; set; }

        /// <summary>Recent attendance marks, most recent first.</summary>
        public List<AttendanceHistoryItem> AttendanceHistory { get; set; } = new();

        /// <summary>Total days marked across all statuses.</summary>
        public int TotalMarked => PresentCount + AbsentCount + LateCount + LeaveCount;

        /// <summary>
        /// Attendance rate as a whole percentage (Present and Late both count as
        /// attended), or null when nothing has been marked yet.
        /// </summary>
        public int? AttendancePercent =>
            TotalMarked == 0 ? null : (int)Math.Round((PresentCount + LateCount) * 100.0 / TotalMarked);

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
