namespace Scholar.Models.ViewModels
{
    public class TeacherRow
    {
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string Subjects { get; set; } = string.Empty;

        public string Classes { get; set; } = string.Empty;
    }
}
