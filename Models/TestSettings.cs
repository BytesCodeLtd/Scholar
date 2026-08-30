using Scholar.Enums;

namespace Scholar.Models
{
    public class TestSettings : IAuditableEntity
    {
        public int Id { get; set; }

        public int InstituteId { get; set; }

        public Institute Institute { get; set; } = null!;

        public int HeaderLayout { get; set; } = 1;

        public string HeaderFontStyle { get; set; } = "Default";

        public int HeaderFontSize { get; set; } = 40;

        public int HeadingFontSize { get; set; } = 16;

        public int TextFontSize { get; set; } = 12;

        public TextFormatting TextFormatting { get; set; } = TextFormatting.Normal;

        public TextFormatting HeadingFormatting { get; set; } = TextFormatting.Bold;

        public decimal LineHeight { get; set; } = 2.5m;

        public PaperFontColor PaperFontColor { get; set; } = PaperFontColor.Black;

        public string EnglishTextFontStyle { get; set; } = "Default";

        public WatermarkType Watermark { get; set; } = WatermarkType.None;

        public decimal PicWatermarkOpacity { get; set; } = 0.4m;

        public int PicWatermarkHeight { get; set; } = 60;

        public int PicWatermarkWidth { get; set; } = 70;

        public int LogoHeight { get; set; } = 100;

        public int LogoWidth { get; set; } = 100;

        public McqsLayout McqsLayout { get; set; } = McqsLayout.Default;

        public NumeralStyle NumeralStyle { get; set; } = NumeralStyle.Roman;

        public string? ImportantNote { get; set; }

        public string? FooterText { get; set; }

        public bool ShowConceptualQuestionMark { get; set; }

        public bool HidePhoneNumber { get; set; }

        public bool QuestionHeadingBottomBorder { get; set; }

        public bool ShowChapterName { get; set; }

        public bool ShowGrammaticalTerms { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
