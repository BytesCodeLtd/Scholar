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
        public PaperSettingsInput? Settings { get; set; }
    }

    public class RenderSectionsRequest
    {
        public int? InstituteId { get; set; }
        public List<PaperSectionInput> Sections { get; set; } = [];
    }

    public class RenderCanvasRequest
    {
        public int? InstituteId { get; set; }
        public string? Title { get; set; }
        public string? PaperType { get; set; }
        public int DurationMinutes { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public PaperSettingsInput? Settings { get; set; }
        public List<PaperSectionInput> Sections { get; set; } = [];
    }

    public class PaperSettingsInput
    {
        public int HeaderLayout { get; set; } = 1;
        public string HeaderFontStyle { get; set; } = "Default";
        public int HeaderFontSize { get; set; } = 40;
        public int HeadingFontSize { get; set; } = 16;
        public int TextFontSize { get; set; } = 12;
        public TextFormatting TextFormatting { get; set; } = TextFormatting.Normal;
        public TextFormatting HeadingFormatting { get; set; } = TextFormatting.Bold;
        public decimal LineHeight { get; set; } = 2.5m;
        public string FontColorHex { get; set; } = "#000000";
        public string EnglishTextFontStyle { get; set; } = "Default";

        public WatermarkType Watermark { get; set; } = WatermarkType.None;
        public decimal PicWatermarkOpacity { get; set; } = 0.4m;
        public int PicWatermarkHeight { get; set; } = 60;
        public int PicWatermarkWidth { get; set; } = 70;
        public int LogoHeight { get; set; } = 90;
        public int LogoWidth { get; set; } = 90;

        public McqsLayout McqsLayout { get; set; } = McqsLayout.Default;
        public NumeralStyle NumeralStyle { get; set; } = NumeralStyle.Roman;
        public string? ImportantNote { get; set; }
        public string? FooterText { get; set; }

        public bool ShowConceptualQuestionMark { get; set; }
        public bool HidePhoneNumber { get; set; }
        public bool QuestionHeadingBottomBorder { get; set; }
        public bool ShowChapterName { get; set; }
        public bool ShowGrammaticalTerms { get; set; }

        public void ApplyTo(PaperRenderSettings s)
        {
            s.HeaderLayout = HeaderLayout;
            s.HeaderFontStyle = HeaderFontStyle;
            s.HeaderFontSize = HeaderFontSize;
            s.HeadingFontSize = HeadingFontSize;
            s.TextFontSize = TextFontSize;
            s.TextFormatting = TextFormatting;
            s.HeadingFormatting = HeadingFormatting;
            s.LineHeight = LineHeight;
            s.FontColorHex = string.IsNullOrWhiteSpace(FontColorHex) ? "#000000" : FontColorHex;
            s.EnglishTextFontStyle = EnglishTextFontStyle;
            s.Watermark = Watermark;
            s.PicWatermarkOpacity = PicWatermarkOpacity;
            s.PicWatermarkHeight = PicWatermarkHeight;
            s.PicWatermarkWidth = PicWatermarkWidth;
            s.LogoHeight = LogoHeight;
            s.LogoWidth = LogoWidth;
            s.McqsLayout = McqsLayout;
            s.NumeralStyle = NumeralStyle;
            s.ImportantNote = ImportantNote;
            s.FooterText = FooterText;
            s.ShowConceptualQuestionMark = ShowConceptualQuestionMark;
            s.HidePhoneNumber = HidePhoneNumber;
            s.QuestionHeadingBottomBorder = QuestionHeadingBottomBorder;
            s.ShowChapterName = ShowChapterName;
            s.ShowGrammaticalTerms = ShowGrammaticalTerms;
        }
    }

    public class PaperSectionInput
    {
        public QuestionType Type { get; set; }
        public string? Instruction { get; set; }
        public int MarksPerQuestion { get; set; }
        public List<int> QuestionIds { get; set; } = new();
    }
}
