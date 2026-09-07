namespace Scholar.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTests { get; set; }

        public int SavedTests { get; set; }

        public int TestsThisMonth { get; set; }

        public int TotalMcqs { get; set; }

        public int TotalShortQuestions { get; set; }

        public int TotalLongQuestions { get; set; }

        public int TotalQuestions => TotalMcqs + TotalShortQuestions + TotalLongQuestions;

        public bool IsSuperAdmin { get; set; }

        public int TotalInstitutes { get; set; }

        public int ActiveInstitutes { get; set; }

        public int InstitutesThisMonth { get; set; }

        public int TotalStudents { get; set; }

        public int StudentsThisMonth { get; set; }
    }
}
