using System.ComponentModel.DataAnnotations;

namespace Scholar.Enums
{
    public enum TextFormatting
    {
        Normal,
        Bold,
        Italic,
        [Display(Name = "Small Caps")]
        SmallCaps
    }
}
