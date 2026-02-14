using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;

namespace Application.Interfaces;

public interface IS3Service
{
    Task<bool> DeleteFile(string key);
    Uri BuildPublicUri(string chaveObjeto);
    Uri GenerateURL(string key);
    Task<Result<byte[]>> GetFileByKey(string key);
    Task<bool> SaveFile(byte[] file, string key, string contentType, bool cannedACL = false);
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
    Task UploadImageAsync(Stream imageStream, string folder, string fileName, string contentType);
}
