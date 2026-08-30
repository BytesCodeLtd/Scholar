namespace Scholar.Common.Paging
{
    public class PageParameters
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Filter { get; set; }

        public string Search { get; set; } = string.Empty;

        public string OrderBy { get; set; } = "Id desc";
    }
}
