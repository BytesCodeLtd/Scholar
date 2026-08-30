namespace Scholar.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTests { get; set; }

        public int TestsThisMonth { get; set; }

        public bool IsSuperAdmin { get; set; }

        public int TotalInstitutes { get; set; }

        public int InstitutesThisMonth { get; set; }
    }
}
