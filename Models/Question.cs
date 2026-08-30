using Scholar.Enums;

namespace Scholar.Models
{
    /// <summary>
    /// A single question in the bank. Shared when <see cref="OwnerId"/> is null;
    /// otherwise it belongs privately to that teacher.
    /// </summary>
    public class Question : IAuditableEntity
    {
        public int Id { get; set; }

        public int TopicId { get; set; }

        public Topic Topic { get; set; } = null!;

        public QuestionType Type { get; set; }

        public string Text { get; set; } = string.Empty;

        public int Marks { get; set; } = 1;

        public Difficulty Difficulty { get; set; } = Difficulty.Medium;

        public QuestionCategory Category { get; set; } = QuestionCategory.Exercise;

        public string? OwnerId { get; set; }

        public ApplicationUser? Owner { get; set; }

        public ICollection<McqOption> Options { get; set; } = new List<McqOption>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class McqOption : IAuditableEntity
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; } = null!;

        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
