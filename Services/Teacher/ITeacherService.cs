using Scholar.Common.Paging;
using Scholar.Models.ViewModels;

namespace Scholar.Services
{
    public interface ITeacherService
    {
        Task<PagedResult<TeacherRow>> GetTeachersAsync(PageParameters tableParams);
    }
}
