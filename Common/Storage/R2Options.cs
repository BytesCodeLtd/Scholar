namespace Scholar.Common.Storage
{
    public class R2Options
    {
        public const string SectionName = "Storage:R2";

        public string AccountId { get; set; } = string.Empty;

        public string AccessKey { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public string BucketName { get; set; } = string.Empty;

        public string PublicBaseUrl { get; set; } = string.Empty;

        public long MaxFileSizeBytes { get; set; } = 2 * 1024 * 1024;

        public string ServiceUrl => $"https://{AccountId}.r2.cloudflarestorage.com";
    }
}
