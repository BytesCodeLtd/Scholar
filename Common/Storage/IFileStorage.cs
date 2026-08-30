namespace Scholar.Common.Storage
{
    /// <summary>
    /// Abstraction over blob storage so callers never depend on a specific
    /// provider. The current implementation targets Cloudflare R2, but any
    /// S3-compatible backend can be swapped in without touching controllers.
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Uploads a file and returns its publicly accessible URL. The URL is
        /// what gets persisted on the owning entity (e.g. Institute.LogoUrl).
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        /// <param name="keyPrefix">Logical folder inside the bucket, e.g. "logos".</param>
        Task<string> UploadAsync(IFormFile file, string keyPrefix, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a previously uploaded file given the public URL returned by
        /// <see cref="UploadAsync"/>. No-op if the URL is empty or not ours.
        /// </summary>
        Task DeleteAsync(string? fileUrl, CancellationToken cancellationToken = default);
    }
}
