using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace Scholar.Common.Storage
{
    public partial class R2FileStorage : IFileStorage
    {
        private const int MaxSlugLength = 60;
        private readonly IAmazonS3 _client;
        private readonly R2Options _options;

        public R2FileStorage(IAmazonS3 client, IOptions<R2Options> options)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<string> UploadAsync(IFormFile file, string keyPrefix, CancellationToken cancellationToken = default)
        {
            if (file is null || file.Length == 0)
            {
                throw new ArgumentException("No file was provided to upload.", nameof(file));
            }

            string safeName = Path.GetFileName(file.FileName);
            string extension = Path.GetExtension(safeName).ToLowerInvariant();
            string slug = Slugify(Path.GetFileNameWithoutExtension(safeName));

            string namePart = string.IsNullOrEmpty(slug)
                ? $"{Guid.NewGuid():N}"
                : $"{slug}-{Guid.NewGuid():N}";

            string key = $"{keyPrefix.Trim('/')}/{namePart}{extension}";

            await using Stream stream = file.OpenReadStream();

            PutObjectRequest request = new()
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType,
                DisablePayloadSigning = true
            };

            await _client.PutObjectAsync(request, cancellationToken);

            return $"{_options.PublicBaseUrl.TrimEnd('/')}/{key}";
        }

        public async Task DeleteAsync(string? fileUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return;
            }

            string prefix = _options.PublicBaseUrl.TrimEnd('/') + "/";

            if (!fileUrl.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string key = fileUrl[prefix.Length..];

            await _client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            }, cancellationToken);
        }

        private static string Slugify(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string lower = value.ToLowerInvariant();
            string collapsed = NonSlugChars().Replace(lower, "-").Trim('-');

            if (collapsed.Length > MaxSlugLength)
            {
                collapsed = collapsed[..MaxSlugLength].Trim('-');
            }

            return collapsed;
        }

        [GeneratedRegex("[^a-z0-9]+")]
        private static partial Regex NonSlugChars();
    }
}
