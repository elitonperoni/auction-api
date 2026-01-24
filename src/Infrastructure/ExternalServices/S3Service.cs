using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces;
using Domain.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.ExternalServices;

public class S3Service(IAmazonS3 s3Client, IOptions<AwsConfig> awsOptions) : IS3Service
{
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = awsOptions.Value.BucketName,
            Key = fileName, 
            InputStream = fileStream,
            AutoCloseStream = true
        };

        _ = await s3Client.PutObjectAsync(putRequest);

        return $"Arquivo {fileName} enviado com sucesso!";
    }

    public async Task UploadImageAsync(Stream imageStream, string folder, string fileName, string contentType)
    {
        var originalPutRequest = new PutObjectRequest
        {
            BucketName = awsOptions.Value.BucketName,
            Key = $"{folder}/{fileName}",
            InputStream = imageStream,
            ContentType = $"image/{contentType}"
        };

        await s3Client.PutObjectAsync(originalPutRequest);
    }

    public Uri GenerateURL(string key)
    {
        return new Uri($"https://{awsOptions.Value.BucketName}.s3.{awsOptions.Value.Region}.amazonaws.com/{key}");
    }

    public Uri GeneratePublicURL(string chaveObjeto)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = awsOptions.Value.BucketName,
            Key = chaveObjeto,
            Expires = DateTime.UtcNow.AddHours(1) 
        };

        return new Uri(s3Client.GetPreSignedURL(request));
    }

    public async Task<Result<byte[]>> GetFileByKey(string key)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = awsOptions.Value.BucketName,
                Key = key
            };

            using GetObjectResponse getResponse = await s3Client.GetObjectAsync(request);

            if (getResponse?.ResponseStream != null)
            {
                using MemoryStream ms = new();
                await getResponse.ResponseStream.CopyToAsync(ms);

                return Result<byte[]>.Success(ms.ToArray());
            }
            return Result.Failure<byte[]>(new Error("", "", ErrorType.NotFound));
        }
        catch
        {
            return Result.Failure<byte[]>(new Error("Not found", "Not found", ErrorType.NotFound));
        }
    }

    public async Task<bool> SaveFile(byte[] file, string key, string contentType, bool cannedACL = false)
    {
        try
        {
            using var stream = new MemoryStream(file);
            PutObjectRequest request = new()
            {
                BucketName = awsOptions.Value.BucketName,
                InputStream = stream,
                ContentType = contentType,
                Key = key
            };

            if (cannedACL)
            {
                request.CannedACL = S3CannedACL.PublicRead;
            }

            await s3Client.PutObjectAsync(request);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteFile(string key)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = awsOptions.Value.BucketName,
                Key = key
            };

            DeleteObjectResponse response = await s3Client.DeleteObjectAsync(request);

            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (Exception)
        {
            return false;
        }
    }

}
