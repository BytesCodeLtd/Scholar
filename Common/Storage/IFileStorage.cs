namespace Scholar.Common.Storage
{
    public interface IFileStorage
    {
        Task<string> UploadAsync(IFormFile file, string keyPrefix, CancellationToken cancellationToken = default);

        Task DeleteAsync(string? fileUrl, CancellationToken cancellationToken = default);
    }
}
