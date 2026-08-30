using Scholar.Enums;

namespace Scholar.Models
{
    public class Test : IAuditableEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public int SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;

        public int TotalMarks { get; set; }

        public int DurationMinutes { get; set; }

        public string? HeaderNotes { get; set; }

        public string? PaperSettingsSnapshot { get; set; }

        public ICollection<TestSection> Sections { get; set; } = new List<TestSection>();

        public ICollection<TestQuestion> Questions { get; set; } = new List<TestQuestion>();

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class TestSection : IAuditableEntity
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        public Test Test { get; set; } = null!;

        public int Order { get; set; }

        public QuestionType Type { get; set; }

        public string? Instruction { get; set; }

        public int MarksPerQuestion { get; set; }

        public ICollection<TestQuestion> Questions { get; set; } = [];

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class TestQuestion : IAuditableEntity
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        public Test Test { get; set; } = null!;

        public int QuestionId { get; set; }

        public Question Question { get; set; } = null!;

        public int Order { get; set; }

        public int? TestSectionId { get; set; }

        public TestSection? TestSection { get; set; }

        public PaperSection Section { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
