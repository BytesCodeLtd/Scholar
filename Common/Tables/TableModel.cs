using Scholar.Common.Paging;
using Scholar.Enums;

namespace Scholar.Common.Tables
{
    public class TableColumn
    {
        public string Header { get; set; } = string.Empty;

        public string Property { get; set; } = string.Empty;

        public bool Sortable { get; set; }
    }

    public class TableAction
    {
        public string Label { get; set; } = string.Empty;

        public string? Controller { get; set; }

        public string? Action { get; set; }

        public IDictionary<string, string?>? RouteValues { get; set; }

        public string? Url { get; set; }

        public string Method { get; set; } = "get";

        public TableActionStyle Style { get; set; } = TableActionStyle.Primary;

        public string? IconSvg { get; set; }

        public string? Confirm { get; set; }
    }

    public class RowMenuItem
    {
        public string Label { get; set; } = string.Empty;

        public string? Controller { get; set; }

        public string Action { get; set; } = string.Empty;

        public string RouteKey { get; set; } = "id";

        public IDictionary<string, string?>? RouteValues { get; set; }

        public string Method { get; set; } = "get";

        public RowMenuItemStyle Style { get; set; } = RowMenuItemStyle.Default;

        public string? IconSvg { get; set; }

        public string? Confirm { get; set; }

        public string? VisibleWhenProperty { get; set; }

        public bool VisibleWhenValue { get; set; } = true;
    }

    public class TableModel
    {
        public IReadOnlyList<object> Rows { get; init; } = [];

        public List<TableColumn> Columns { get; init; } = [];

        public int Page { get; init; }

        public int PageSize { get; init; }

        public int TotalCount { get; init; }

        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

        public bool ShowCheckbox { get; init; }

        public string? CheckboxValueProperty { get; init; }

        public bool ShowSearch { get; init; }

        public bool ShowRowMenu { get; init; }

        public string? RowKeyProperty { get; init; }

        public List<RowMenuItem> RowMenuItems { get; init; } = [];

        public string? Title { get; init; }

        public List<TableAction> Actions { get; init; } = [];

        public object? Cell(object row, TableColumn column) => row.GetType().GetProperty(column.Property)?.GetValue(row);

        public object? CheckboxValue(object row) =>
            CheckboxValueProperty is null ? null : row.GetType().GetProperty(CheckboxValueProperty)?.GetValue(row);

        public object? RowMenuValue(object row) =>
            RowKeyProperty is null ? null : row.GetType().GetProperty(RowKeyProperty)?.GetValue(row);

        public bool IsRowMenuItemVisible(RowMenuItem item, object row)
        {
            if (item.VisibleWhenProperty is null)
            {
                return true;
            }

            object? value = row.GetType().GetProperty(item.VisibleWhenProperty)?.GetValue(row);
            return value is bool flag && flag == item.VisibleWhenValue;
        }

        public static TableModelBuilder<T> For<T>(PagedResult<T> paged) => new(paged);
    }

    public class TableModelBuilder<T>
    {
        private readonly PagedResult<T> _paged;

        private readonly List<TableColumn> _columns = [];

        private readonly List<TableAction> _actions = [];

        private readonly List<RowMenuItem> _rowMenuItems = [];

        private bool _showCheckbox;
        private string? _checkboxValueProperty;
        private bool _showSearch;
        private bool _showRowMenu;
        private string? _rowKeyProperty;
        private string? _title;

        public TableModelBuilder(PagedResult<T> paged) => _paged = paged;

        public TableModelBuilder<T> Title(string title)
        {
            _title = title;
            return this;
        }

        public TableModelBuilder<T> Column(string header, string property, bool sortable = false)
        {
            _columns.Add(new TableColumn { Header = header, Property = property, Sortable = sortable });
            return this;
        }

        public TableModelBuilder<T> WithCheckbox(bool show = true, string? valueProperty = null)
        {
            _showCheckbox = show;
            _checkboxValueProperty = valueProperty;
            return this;
        }

        public TableModelBuilder<T> WithSearch(bool show = true)
        {
            _showSearch = show;
            return this;
        }

        public TableModelBuilder<T> WithRowMenu(bool show = true)
        {
            _showRowMenu = show;
            return this;
        }

        public TableModelBuilder<T> WithRowMenu(string keyProperty, params RowMenuItem[] items)
        {
            _showRowMenu = true;
            _rowKeyProperty = keyProperty;
            _rowMenuItems.AddRange(items);
            return this;
        }

        public TableModelBuilder<T> WithAction(TableAction action)
        {
            _actions.Add(action);
            return this;
        }

        public TableModelBuilder<T> WithAction(
            string label,
            string action,
            string? controller = null,
            TableActionStyle style = TableActionStyle.Primary,
            string method = "get",
            IDictionary<string, string?>? routeValues = null,
            string? iconSvg = null,
            string? confirm = null)
        {
            _actions.Add(new TableAction
            {
                Label = label,
                Action = action,
                Controller = controller,
                Style = style,
                Method = method,
                RouteValues = routeValues,
                IconSvg = iconSvg,
                Confirm = confirm
            });
            return this;
        }

        public TableModel Build() => new()
        {
            Rows = [.. _paged.Items.Cast<object>()],
            Columns = _columns,
            Page = _paged.Page,
            PageSize = _paged.PageSize,
            TotalCount = _paged.TotalCount,
            ShowCheckbox = _showCheckbox,
            CheckboxValueProperty = _checkboxValueProperty,
            ShowSearch = _showSearch,
            ShowRowMenu = _showRowMenu,
            RowKeyProperty = _rowKeyProperty,
            RowMenuItems = _rowMenuItems,
            Title = _title,
            Actions = _actions
        };
    }
}
