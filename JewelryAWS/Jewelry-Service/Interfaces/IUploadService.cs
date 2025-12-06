using Microsoft.AspNetCore.Http;

namespace Jewelry_Service.Interfaces;

public interface IUploadService
{
    Task<string> UploadImage(IFormFile file);
    Task<string> GetPresignedUrl(string key);
    Task<bool> DeleteImage(string key);
}