namespace Scholar.Models
{
    public class PastPaper : IAuditableEntity
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;

        public string Title { get; set; } = string.Empty;

        public int? Year { get; set; }

        public string FileUrl { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
