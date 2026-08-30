namespace Scholar.Common.Storage
{
    public class R2Options
    {
        public const string SectionName = "Storage:R2";

        /// <summary>Cloudflare account id; forms the S3 endpoint host.</summary>
        public string AccountId { get; set; } = string.Empty;

        /// <summary>R2 API token access key id.</summary>
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>R2 API token secret access key.</summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>Target bucket name.</summary>
        public string BucketName { get; set; } = string.Empty;

        /// <summary>
        /// Public base URL for objects, without a trailing slash — either the
        /// bucket's r2.dev dev URL or a custom domain bound to the bucket.
        /// e.g. "https://pub-xxxx.r2.dev".
        /// </summary>
        public string PublicBaseUrl { get; set; } = string.Empty;

        /// <summary>Max accepted upload size in bytes. Defaults to 2 MB.</summary>
        public long MaxFileSizeBytes { get; set; } = 2 * 1024 * 1024;

        /// <summary>The S3-compatible endpoint R2 exposes for this account.</summary>
        public string ServiceUrl => $"https://{AccountId}.r2.cloudflarestorage.com";
    }
}
