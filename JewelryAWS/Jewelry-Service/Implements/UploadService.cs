using Amazon.S3;
using Amazon.S3.Model;
using Jewelry_Model.Settings;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Jewelry_Service.Implements;

public class UploadService : IUploadService
{
    private readonly S3Settings _s3Settings;
    private readonly IAmazonS3 _s3Client;

    public UploadService(IOptions<S3Settings> s3Settings, IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
        _s3Settings = s3Settings.Value;
    }

    public async Task<string> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Only image files are allowed (.jpg, .jpeg, .png, .gif, .bmp, .webp).");

        if (string.IsNullOrEmpty(file.FileName))
            throw new ArgumentException("File name cannot be empty.");
        if (string.IsNullOrEmpty(file.ContentType))
            throw new ArgumentException("File content type cannot be empty.");

        using var stream = file.OpenReadStream();

        var key = Guid.NewGuid();
        var putRequest = new PutObjectRequest
        {
            BucketName = _s3Settings.BucketName,
            Key = $"images/{key}",
            InputStream = stream,
            ContentType = file.ContentType,
            Metadata =
            {
                ["file-name"] = file.FileName
            }
        };

        await _s3Client.PutObjectAsync(putRequest);
        return key.ToString();
    }

    public async Task<string> GetPresignedUrl(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _s3Settings.BucketName,
            Key = $"images/{key}",
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<bool> DeleteImage(string key)
    {
        var getRequest = new DeleteObjectRequest
        {
            BucketName = _s3Settings.BucketName,
            Key = $"images/{key}"
        };

        var deleteResponse = await _s3Client.DeleteObjectAsync(getRequest);
        return deleteResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }
}