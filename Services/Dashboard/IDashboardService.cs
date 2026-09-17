using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> BuildDashboardAsync();

        Task<bool> SaveLayoutAsync(string? layout);
    }
}
