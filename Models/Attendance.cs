using Scholar.Enums;

namespace Scholar.Models
{
    /// <summary>
    /// A single day's attendance mark for one student. There is at most one row
    /// per student per date (enforced by a unique index); re-marking a day updates
    /// the existing row. InstituteId is denormalised from the student for fast,
    /// institute-scoped roster queries.
    /// </summary>
    public class Attendance : IAuditableEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public int StudentId { get; set; }

        public Student Student { get; set; } = null!;

        /// <summary>The calendar day this mark applies to (date only, no time).</summary>
        public DateTime Date { get; set; }

        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
