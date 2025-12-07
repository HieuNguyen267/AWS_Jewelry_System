

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using Jewelry_Model.Settings;
using Jewelry_Service.AwsS3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Jewelry_Service.AwsS3.Services
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Models.S3Object s3obj);
    }

    public class StorageService : IStorageService
    {
        private readonly IAmazonS3 _amazonS3Client;
        public StorageService(IAmazonS3 amazonS3Client)
        {
            _amazonS3Client = amazonS3Client;
        }
        public async Task<string> UploadFileAsync(Models.S3Object s3obj)
        {
            string key = "";


            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = s3obj.InputStream,
                Key = s3obj.Name,
                BucketName = s3obj.BucketName,
                CannedACL = S3CannedACL.NoACL
            };

            var transferUtility = new TransferUtility(_amazonS3Client);

            await transferUtility.UploadAsync(uploadRequest);
            key = s3obj.Name;
            return key;


        }


    }

}
