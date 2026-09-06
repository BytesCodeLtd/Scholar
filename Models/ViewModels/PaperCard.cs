namespace Scholar.Models.ViewModels
{
    /// <summary>A saved paper (test) summarised for the cards on the Saved Papers page.</summary>
    public class PaperCard
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public string Subject { get; set; } = string.Empty;

        // Owning institute — shown on the cards when a super admin views all papers.
        public string Institute { get; set; } = string.Empty;

        public int TotalMarks { get; set; }

        public int DurationMinutes { get; set; }

        public int QuestionCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
