using Scholar.Constants;
using Scholar.Enums;

namespace Scholar.Common.Tables
{
    public static class TableActions
    {
        public static TableAction AddInstitute() => new()
        {
            Label = "+",
            Controller = ControllerNames.Institute,
            Action = "CreateOrUpdate",
            Style = TableActionStyle.Primary
        };

        public static TableAction AddStudent() => new()
        {
            Label = "+",
            Controller = ControllerNames.Student,
            Action = "Create",
            Style = TableActionStyle.Primary
        };

        public static TableAction[] Attendance(string date, int gradeId, string? section, int? instituteId, int page)
        {
            TableAction Btn(AttendanceStatus status, string css) => new()
            {
                Label = status.ToString(),
                Controller = ControllerNames.Attendance,
                Action = "MarkAll",
                Method = "post",
                Css = css,
                Confirm = $"Mark the whole class {status} on {date}?",
                RouteValues = new Dictionary<string, string?>
                {
                    ["status"] = status.ToString(),
                    ["date"] = date,
                    ["gradeId"] = gradeId.ToString(),
                    ["section"] = section,
                    ["instituteId"] = instituteId?.ToString(),
                    ["page"] = page.ToString()
                }
            };

            return
            [
                Btn(AttendanceStatus.Present, "text-white bg-green-600 border border-green-600 hover:bg-green-700"),
                Btn(AttendanceStatus.Absent, "text-white bg-red-600 border border-red-600 hover:bg-red-700"),
                Btn(AttendanceStatus.Late, "text-white bg-amber-500 border border-amber-500 hover:bg-amber-600"),
                Btn(AttendanceStatus.Leave, "text-white bg-blue-600 border border-blue-600 hover:bg-blue-700")
            ];
        }
    }
}
