using Scholar.Constants;
using Scholar.Enums;

namespace Scholar.Common.Tables
{
    public static class RowMenus
    {
        public static RowMenuItem Edit(string controller, string action = "CreateOrUpdate") => new()
        {
            Label = "Edit",
            Controller = controller,
            Action = action,
            IconSvg = Icons.Pencil
        };

        public static RowMenuItem View(string controller, string action = "Details") => new()
        {
            Label = "View",
            Controller = controller,
            Action = action,
            IconSvg = Icons.Eye
        };

        public static RowMenuItem Delete(string controller, string action = "Delete") => new()
        {
            Label = "Delete",
            Controller = controller,
            Action = action,
            Method = "post",
            Style = RowMenuItemStyle.Danger,
            Confirm = "Delete this record? This can't be undone.",
            IconSvg = Icons.Trash
        };

        // Soft-delete pair for auditable records: only one shows per row based on IsActive.
        public static RowMenuItem Deactivate(string controller, string action = "Delete") => new()
        {
            Label = "Delete",
            Controller = controller,
            Action = action,
            Method = "post",
            Style = RowMenuItemStyle.Danger,
            Confirm = "Delete this record?",
            IconSvg = Icons.Trash,
            VisibleWhenProperty = "IsActive",
            VisibleWhenValue = true
        };

        public static RowMenuItem Activate(string controller, string action = "Activate") => new()
        {
            Label = "Activate",
            Controller = controller,
            Action = action,
            Method = "post",
            IconSvg = Icons.Check,
            VisibleWhenProperty = "IsActive",
            VisibleWhenValue = false
        };

        public static RowMenuItem[] Institute() =>
        [
            Edit(ControllerNames.Institute, "CreateOrUpdate"),
            Deactivate(ControllerNames.Institute),
            Activate(ControllerNames.Institute)
        ];

        public static RowMenuItem[] Student() =>
        [
            View(ControllerNames.Student),
            Edit(ControllerNames.Student),
            Deactivate(ControllerNames.Student),
            Activate(ControllerNames.Student)
        ];

        public static RowMenuItem[] Teacher() =>
        [
            Edit(ControllerNames.Teacher),
            Delete(ControllerNames.Teacher)
        ];

        // Small inline icons (Flowbite-style, sized w-4 h-4) shown beside each label.
        private static class Icons
        {
            public const string Pencil =
                "<svg class=\"w-4 h-4\" aria-hidden=\"true\" xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\">" +
                "<path stroke=\"currentColor\" stroke-linecap=\"round\" stroke-linejoin=\"round\" stroke-width=\"2\" " +
                "d=\"m14.3 4.8 2.9 2.9M7 7H4a1 1 0 0 0-1 1v10a1 1 0 0 0 1 1h11a1 1 0 0 0 1-1v-4.5m2.4-10a2 2 0 0 1 0 3l-6.8 6.8L8 14l.7-3.6 6.9-6.8a2 2 0 0 1 2.8 0Z\"/></svg>";

            public const string Eye =
                "<svg class=\"w-4 h-4\" aria-hidden=\"true\" xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\">" +
                "<path stroke=\"currentColor\" stroke-width=\"2\" d=\"M21 12c0 1.2-4 6-9 6s-9-4.8-9-6 4-6 9-6 9 4.8 9 6Z\"/>" +
                "<path stroke=\"currentColor\" stroke-width=\"2\" d=\"M12 14a2 2 0 1 0 0-4 2 2 0 0 0 0 4Z\"/></svg>";

            public const string Trash =
                "<svg class=\"w-4 h-4\" aria-hidden=\"true\" xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\">" +
                "<path stroke=\"currentColor\" stroke-linecap=\"round\" stroke-linejoin=\"round\" stroke-width=\"2\" " +
                "d=\"M5 7h14m-9 3v8m4-8v8M10 3h4a1 1 0 0 1 1 1v3H9V4a1 1 0 0 1 1-1ZM6 7h12v13a1 1 0 0 1-1 1H7a1 1 0 0 1-1-1V7Z\"/></svg>";

            public const string Check =
                "<svg class=\"w-4 h-4\" aria-hidden=\"true\" xmlns=\"http://www.w3.org/2000/svg\" fill=\"none\" viewBox=\"0 0 24 24\">" +
                "<path stroke=\"currentColor\" stroke-linecap=\"round\" stroke-linejoin=\"round\" stroke-width=\"2\" " +
                "d=\"M5 11.9 9.2 16 19 6\"/></svg>";
        }
    }
}
