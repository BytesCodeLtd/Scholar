namespace Scholar.Common
{
    /// <summary>Currency formatting helpers. The platform bills in Pakistani Rupees.</summary>
    public static class Money
    {
        public static string Pkr(decimal amount) => $"Rs. {amount:N0}";
    }
}
