using System.ComponentModel.DataAnnotations;

namespace Scholar.Models
{
    public class Board : IAuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Board Name")]
        public string Name { get; set; } = string.Empty; // e.g. "FBISE", "Punjab Board"

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // Grade is a global, board-independent class level (e.g. "9th"). A subject ties a
    // grade to a specific board.
    public class Grade : IAuditableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // e.g. "9th", "10th", "1st Year", "2nd Year"

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Subject : IAuditableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // e.g. "Physics", "Chemistry etc.

        public int BoardId { get; set; }

        public Board Board { get; set; } = null!;

        public int GradeId { get; set; }

        public Grade Grade { get; set; } = null!;

        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Chapter : IAuditableEntity
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public string Name { get; set; } = string.Empty;

        public int SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;

        public ICollection<Topic> Topics { get; set; } = new List<Topic>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Topic : IAuditableEntity
    {
        public int Id { get; set; }

        public string Number { get; set; } = string.Empty; // e.g. "1.1"

        public string Name { get; set; } = string.Empty; // e.g. "Biology and its Branches"

        public int ChapterId { get; set; }

        public Chapter Chapter { get; set; } = null!;

        public ICollection<Question> Questions { get; set; } = new List<Question>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
