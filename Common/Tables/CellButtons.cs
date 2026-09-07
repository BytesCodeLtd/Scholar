using Scholar.Constants;
using Scholar.Enums;

namespace Scholar.Common.Tables
{
    public static class CellButtons
    {
        public static List<TableCellButton> Attendance(string date, int gradeId, string? section, int? instituteId, int page)
        {
            const string inactive = "border border-gray-300 text-gray-600 hover:bg-gray-100";

            TableCellButton Btn(AttendanceStatus status, string activeCss) => new()
            {
                Text = status.ToString(),
                Value = status.ToString(),
                Controller = ControllerNames.Attendance,
                Action = "Mark",
                Method = "post",
                RouteKey = "studentId",
                RouteValues = new Dictionary<string, string?>
                {
                    ["status"] = status.ToString(),
                    ["date"] = date,
                    ["gradeId"] = gradeId.ToString(),
                    ["section"] = section,
                    ["instituteId"] = instituteId?.ToString(),
                    ["page"] = page.ToString()
                },
                ActiveCss = activeCss,
                InactiveCss = inactive
            };

            return
            [
                Btn(AttendanceStatus.Present, "bg-green-600 text-white border border-green-600"),
                Btn(AttendanceStatus.Absent, "bg-red-600 text-white border border-red-600"),
                Btn(AttendanceStatus.Late, "bg-amber-500 text-white border border-amber-500"),
                Btn(AttendanceStatus.Leave, "bg-blue-600 text-white border border-blue-600")
            ];
        }
    }
}
