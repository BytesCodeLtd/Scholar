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

        public int TotalPastPapers { get; set; }

        public List<string> InstituteChartLabels { get; set; } = [];

        public List<int> InstituteChartData { get; set; } = [];

        public int TotalStudents { get; set; }

        public int StudentsThisMonth { get; set; }

        public int TotalTeachers { get; set; }

        /// <summary>Month labels (e.g. "Apr") for the tests-generated bar chart.</summary>
        public List<string> TestChartLabels { get; set; } = [];

        /// <summary>Tests generated per month, aligned with <see cref="TestChartLabels"/>.</summary>
        public List<int> TestChartData { get; set; } = [];
    }
}
