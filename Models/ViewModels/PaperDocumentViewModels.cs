using Scholar.Common.Papers;
using Scholar.Enums;

namespace Scholar.Models.ViewModels
{
    public class PaperSectionRenderModel
    {
        public int SectionNumber { get; set; }

        public string? Instruction { get; set; }

        public QuestionType Type { get; set; }

        public int MarksPerQuestion { get; set; }

        public int StartIndex { get; set; }

        public List<Question> Questions { get; set; } = [];

        public int Count => Questions.Count;

        public int TotalMarks => MarksPerQuestion * Count;
    }

    public class PaperSectionsViewModel
    {
        public PaperRenderSettings Settings { get; set; } = new();
        public List<PaperSectionRenderModel> Sections { get; set; } = new();
    }

    public class PaperDocumentViewModel
    {
        public int TestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PaperType { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalMarks { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;

        public PaperRenderSettings Settings { get; set; } = new();
        public List<PaperSectionRenderModel> Sections { get; set; } = new();
    }

    public class SavePaperRequest
    {
        public int SubjectId { get; set; }

        public int? InstituteId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? PaperType { get; set; }
        public int DurationMinutes { get; set; }
        public List<PaperSectionInput> Sections { get; set; } = new();
    }

    public class RenderSectionsRequest
    {
        public int? InstituteId { get; set; }
        public List<PaperSectionInput> Sections { get; set; } = [];
    }

    public class PaperSectionInput
    {
        public QuestionType Type { get; set; }
        public string? Instruction { get; set; }
        public int MarksPerQuestion { get; set; }
        public List<int> QuestionIds { get; set; } = new();
    }
}
