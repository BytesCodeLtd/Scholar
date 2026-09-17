using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IAccountService
    {
        Task<AccountViewModel?> GetProfileAsync();

        Task<bool> UpdateProfileAsync(AccountViewModel model);
    }
}
