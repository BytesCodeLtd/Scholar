using Scholar.Enums;

namespace Scholar.Common.Tables
{
    public static class TableActions
    {
        public static TableAction AddInstitute() => new()
        {
            Label = "Add",
            Controller = "Institute",
            Action = "CreateOrUpdate",
            Style = TableActionStyle.Primary
        };
    }
}
