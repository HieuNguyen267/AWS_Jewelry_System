using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Jewelry_Service.Implements;

public class UploadService : IUploadService
{
    public Task<string> UploadImage(IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetPresignedUrl(string key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteImage(string key)
    {
        throw new NotImplementedException();
    }
}