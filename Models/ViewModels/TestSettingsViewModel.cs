using System.ComponentModel.DataAnnotations;
using Scholar.Common.Papers;
using Scholar.Enums;

namespace Scholar.Models.ViewModels
{
    public class TestSettingsViewModel
    {
        // Which institute these settings belong to.
        public int InstituteId { get; set; }

        // Display-only context.
        public string InstituteName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }

        // Institute-level: address/contact line printed in the paper header.
        [Display(Name = "Institute Address")]
        public string? InstituteAddress { get; set; }

        // Institute switcher (populated for SuperAdmin only).
        public bool CanSwitchInstitute { get; set; }
        public IEnumerable<Institute> Institutes { get; set; } = new List<Institute>();

        // ---- Layout & typography ----------------------------------------

        [Display(Name = "Paper Header Layout")]
        public int HeaderLayout { get; set; } = 1;

        [Display(Name = "Header Font Style")]
        public string HeaderFontStyle { get; set; } = "Default";

        [Display(Name = "Header Font Size")]
        [Range(PaperSettingsCatalog.MinFontSize, PaperSettingsCatalog.MaxFontSize)]
        public int HeaderFontSize { get; set; } = 40;

        [Display(Name = "Heading Font Size")]
        [Range(PaperSettingsCatalog.MinFontSize, PaperSettingsCatalog.MaxFontSize)]
        public int HeadingFontSize { get; set; } = 16;

        [Display(Name = "Text Font Size")]
        [Range(PaperSettingsCatalog.MinFontSize, PaperSettingsCatalog.MaxFontSize)]
        public int TextFontSize { get; set; } = 12;

        [Display(Name = "Text Formatting")]
        public TextFormatting TextFormatting { get; set; } = TextFormatting.Normal;

        [Display(Name = "Heading Formatting")]
        public TextFormatting HeadingFormatting { get; set; } = TextFormatting.Bold;

        [Display(Name = "Line Height")]
        [Range(PaperSettingsCatalog.MinLineHeight, PaperSettingsCatalog.MaxLineHeight)]
        public decimal LineHeight { get; set; } = 2.5m;

        [Display(Name = "Paper Font Color")]
        public PaperFontColor PaperFontColor { get; set; } = PaperFontColor.Black;

        [Display(Name = "English Text Font Style")]
        public string EnglishTextFontStyle { get; set; } = "Default";

        // ---- Watermark & branding ---------------------------------------

        [Display(Name = "Watermark")]
        public WatermarkType Watermark { get; set; } = WatermarkType.None;

        [Display(Name = "Pic Watermark Opacity")]
        [Range(PaperSettingsCatalog.MinOpacity, PaperSettingsCatalog.MaxOpacity)]
        public decimal PicWatermarkOpacity { get; set; } = 0.4m;

        [Display(Name = "Pic Watermark Height")]
        [Range(PaperSettingsCatalog.MinDimension, PaperSettingsCatalog.MaxDimension)]
        public int PicWatermarkHeight { get; set; } = 60;

        [Display(Name = "Pic Watermark Width")]
        [Range(PaperSettingsCatalog.MinDimension, PaperSettingsCatalog.MaxDimension)]
        public int PicWatermarkWidth { get; set; } = 70;

        [Display(Name = "Logo Height")]
        [Range(PaperSettingsCatalog.MinDimension, PaperSettingsCatalog.MaxDimension)]
        public int LogoHeight { get; set; } = 100;

        [Display(Name = "Logo Width")]
        [Range(PaperSettingsCatalog.MinDimension, PaperSettingsCatalog.MaxDimension)]
        public int LogoWidth { get; set; } = 100;

        // ---- Content options --------------------------------------------

        [Display(Name = "MCQs Layout")]
        public McqsLayout McqsLayout { get; set; } = McqsLayout.Default;

        [Display(Name = "Numeral Style")]
        public NumeralStyle NumeralStyle { get; set; } = NumeralStyle.Roman;

        [Display(Name = "Important Note")]
        public string? ImportantNote { get; set; }

        [Display(Name = "Footer Text")]
        public string? FooterText { get; set; }

        // ---- Toggles ----------------------------------------------------

        [Display(Name = "Show Conceptual Question's Mark with Every Question")]
        public bool ShowConceptualQuestionMark { get; set; }

        [Display(Name = "Hide Phone Number on Paper")]
        public bool HidePhoneNumber { get; set; }

        [Display(Name = "Question Heading Bottom Border")]
        public bool QuestionHeadingBottomBorder { get; set; }

        [Display(Name = "Show Chapter Name With Every Question")]
        public bool ShowChapterName { get; set; }

        [Display(Name = "Show Grammatical Terms with MCQs")]
        public bool ShowGrammaticalTerms { get; set; }
    }
}
