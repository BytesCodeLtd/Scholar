using Scholar.Common.Paging;
using Scholar.Enums;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface IFeesService
    {
        bool IsSuperAdmin { get; }

        Task<PagedResult<InvoiceRow>> GetInvoicesAsync(PageParameters tableParams, string? status);

        Task<InvoiceDetailsViewModel?> GetInvoiceDetailsAsync(int id);

        Task<bool> RecordPaymentAsync(int id, decimal amount, DateOnly paidOn, PaymentMethod method, string? note);

        Task<bool> SendReminderAsync(int id);

        Task<GenerateInvoicesViewModel> BuildGenerateFormAsync(int? instituteId);

        Task PopulateGenerateOptionsAsync(GenerateInvoicesViewModel model);

        Task<int?> GenerateAsync(GenerateInvoicesViewModel model);

        Task<PagedResult<FeeCategoryRow>> GetCategoriesAsync(PageParameters tableParams);

        Task<FeeCategoryFormViewModel?> BuildCategoryFormAsync(int? id);

        Task PopulateCategoryOptionsAsync(FeeCategoryFormViewModel model);

        Task<bool> SaveCategoryAsync(FeeCategoryFormViewModel model);

        Task<bool> DeleteCategoryAsync(int id);
    }
}
