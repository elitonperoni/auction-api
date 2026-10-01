namespace Application.Common.Interfaces;

public interface IS3Service
{
    Task<bool> DeleteFile(string key, CancellationToken cancellationToken = default);
    Uri BuildPublicUri(string objectKey);
    Task UploadImageAsync(Stream imageStream, string folder, string fileName, string contentType, CancellationToken cancellationToken = default);
}
