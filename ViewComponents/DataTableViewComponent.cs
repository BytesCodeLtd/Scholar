using Microsoft.AspNetCore.Mvc;
using Scholar.Common.Tables;

namespace Scholar.ViewComponents
{
    public class DataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(TableModel model) => View(model);
    }
}
