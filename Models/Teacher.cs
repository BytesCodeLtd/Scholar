namespace Scholar.Models
{
    public class Teacher : IAuditableEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public int SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;

        public int GradeId { get; set; }

        public Grade Grade { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
