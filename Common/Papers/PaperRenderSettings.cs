using Scholar.Enums;
using Scholar.Models;

namespace Scholar.Common.Papers
{
    public class PaperRenderSettings
    {
        // Institute branding
        public string InstituteName { get; set; } = string.Empty;
        public string? InstituteAddress { get; set; }
        public string? LogoUrl { get; set; }

        // Layout & typography
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

        // Watermark & branding
        public WatermarkType Watermark { get; set; } = WatermarkType.None;
        public decimal PicWatermarkOpacity { get; set; } = 0.4m;
        public int PicWatermarkHeight { get; set; } = 60;
        public int PicWatermarkWidth { get; set; } = 70;
        public int LogoHeight { get; set; } = 100;
        public int LogoWidth { get; set; } = 100;

        // Content options
        public McqsLayout McqsLayout { get; set; } = McqsLayout.Default;
        public NumeralStyle NumeralStyle { get; set; } = NumeralStyle.Roman;
        public string? ImportantNote { get; set; }
        public string? FooterText { get; set; }

        // Toggles
        public bool ShowConceptualQuestionMark { get; set; }
        public bool HidePhoneNumber { get; set; }
        public bool QuestionHeadingBottomBorder { get; set; }
        public bool ShowChapterName { get; set; }
        public bool ShowGrammaticalTerms { get; set; }

        /// <summary>Builds render settings from live settings (or defaults) + institute.</summary>
        public static PaperRenderSettings From(TestSettings s, Institute institute) => new()
        {
            InstituteName = institute.Name,
            InstituteAddress = institute.Address,
            LogoUrl = institute.LogoUrl,
            HeaderLayout = s.HeaderLayout,
            HeaderFontStyle = s.HeaderFontStyle,
            HeaderFontSize = s.HeaderFontSize,
            HeadingFontSize = s.HeadingFontSize,
            TextFontSize = s.TextFontSize,
            TextFormatting = s.TextFormatting,
            HeadingFormatting = s.HeadingFormatting,
            LineHeight = s.LineHeight,
            FontColorHex = PaperSettingsCatalog.ToHex(s.PaperFontColor),
            EnglishTextFontStyle = s.EnglishTextFontStyle,
            Watermark = s.Watermark,
            PicWatermarkOpacity = s.PicWatermarkOpacity,
            PicWatermarkHeight = s.PicWatermarkHeight,
            PicWatermarkWidth = s.PicWatermarkWidth,
            LogoHeight = s.LogoHeight,
            LogoWidth = s.LogoWidth,
            McqsLayout = s.McqsLayout,
            NumeralStyle = s.NumeralStyle,
            ImportantNote = s.ImportantNote,
            FooterText = s.FooterText,
            ShowConceptualQuestionMark = s.ShowConceptualQuestionMark,
            HidePhoneNumber = s.HidePhoneNumber,
            QuestionHeadingBottomBorder = s.QuestionHeadingBottomBorder,
            ShowChapterName = s.ShowChapterName,
            ShowGrammaticalTerms = s.ShowGrammaticalTerms
        };

        /// <summary>CSS font-family value; "Default" falls back to inherit.</summary>
        public string FontFamilyCss(string font) =>
            string.IsNullOrWhiteSpace(font) || font == "Default" ? "inherit" : $"'{font}', sans-serif";

        public string FormattingCss(TextFormatting f) => f switch
        {
            TextFormatting.Bold => "font-weight:700;",
            TextFormatting.Italic => "font-style:italic;",
            TextFormatting.SmallCaps => "font-variant:small-caps;",
            _ => "font-weight:400;"
        };
    }
}
