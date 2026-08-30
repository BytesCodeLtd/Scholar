using System.ComponentModel.DataAnnotations;

namespace Scholar.Enums
{
    public enum NumeralStyle
    {
        [Display(Name = "Roman Numerals")]
        Roman,
        [Display(Name = "Arabic Numerals")]
        Arabic
    }
}
