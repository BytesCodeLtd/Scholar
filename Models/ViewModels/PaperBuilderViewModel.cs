using Scholar.Common.Papers;

namespace Scholar.Models.ViewModels
{
    // Backing model for the Create Paper builder screen.
    public class PaperBuilderViewModel
    {
        public int SubjectId { get; set; }
        public int GradeId { get; set; }

        public string SubjectName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;

        // Institute-branded render settings for the live canvas.
        public PaperRenderSettings Settings { get; set; } = new();

        // Chapter/topic tree for the Question's Menu.
        public IReadOnlyList<Chapter> Chapters { get; set; } = new List<Chapter>();

        // Topics pre-selected from the entry screen (optional).
        public List<int> PreselectedTopicIds { get; set; } = new();
    }
}
