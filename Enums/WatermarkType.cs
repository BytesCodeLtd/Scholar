using System.ComponentModel.DataAnnotations;

namespace Scholar.Enums
{
    public enum WatermarkType
    {
        [Display(Name = "No Watermark")]
        None,
        [Display(Name = "Text Watermark")]
        Text,
        [Display(Name = "Picture Watermark")]
        Picture
    }
}
