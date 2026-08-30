using Scholar.Enums;

namespace Scholar.Common.Papers
{
    public static class PaperSettingsCatalog
    {
        public static readonly IReadOnlyList<(int Number, string Label)> HeaderLayouts = new List<(int, string)>
        {
            (1, "Layout 1: Centered title"),
            (2, "Layout 2: Logo left, title centered"),
            (3, "Layout 3: Logo right, title centered"),
            (4, "Layout 4: Logo & details split"),
            (5, "Layout 5: Compact single line"),
            (6, "Layout 6: Two-line institute banner")
        };

        /// <summary>Font families offered for the header.</summary>
        public static readonly IReadOnlyList<string> HeaderFonts =
        [
            "Default", "Arial", "Times New Roman", "Verdana", "Calibri", "Georgia"
        ];

        /// <summary>Font families offered for English body content.</summary>
        public static readonly IReadOnlyList<string> EnglishFonts =
        [
            "Default", "Arial", "Calibri", "Century Schoolbook", "Georgia",
            "Helvetica", "Times New Roman", "Trebuchet MS", "Verdana"
        ];

        public static string ToHex(PaperFontColor color) => color switch
        {
            PaperFontColor.Black => "#000000",
            PaperFontColor.Green => "#008000",
            PaperFontColor.Khaki => "#8B864E",
            PaperFontColor.SlateGrey => "#708090",
            PaperFontColor.Blue => "#0000FF",
            PaperFontColor.Orange => "#FFA500",
            PaperFontColor.Red => "#FF0000",
            PaperFontColor.Brown => "#8B4513",
            _ => "#000000"
        };

        public const int MinFontSize = 6;
        public const int MaxFontSize = 96;
        public const int MinDimension = 10;
        public const int MaxDimension = 1000;
        public const double MinLineHeight = 1.0;
        public const double MaxLineHeight = 3.0;
        public const double MinOpacity = 0.1;
        public const double MaxOpacity = 1.0;
    }
}
