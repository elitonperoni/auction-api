using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces;
using Domain.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices;

internal sealed partial class S3Service(
    IAmazonS3 s3Client,
    IOptions<AwsConfig> awsOptions,
    ILogger<S3Service> logger) : IS3Service
{
    public async Task UploadImageAsync(
        Stream imageStream,
        string folder,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = awsOptions.Value.BucketName,
            Key = $"{folder}/{fileName}",
            InputStream = imageStream,
            ContentType = $"image/{contentType}"
        };

        await s3Client.PutObjectAsync(request, cancellationToken);
    }

    public Uri BuildPublicUri(string objectKey)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = awsOptions.Value.BucketName,
            Key = objectKey,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        return new Uri(s3Client.GetPreSignedURL(request));
    }

    public async Task<bool> DeleteFile(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = awsOptions.Value.BucketName,
                Key = key
            };

            DeleteObjectResponse response = await s3Client.DeleteObjectAsync(request, cancellationToken);

            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (AmazonS3Exception ex)
        {
            LogDeleteFailed(logger, ex, key);
            return false;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to delete S3 object '{Key}'")]
    private static partial void LogDeleteFailed(ILogger logger, Exception exception, string key);
}
