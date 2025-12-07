

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Jewelry_Model.Settings;
using Jewelry_Service.AwsS3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Jewelry_Service.AwsS3.Services
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(S3Object s3obj);
    }

    public class StorageService : IStorageService
    {
        private readonly IAmazonS3 _amazonS3Client;
        private readonly AwsSettings _awsSettings;
        public StorageService(IAmazonS3 amazonS3Client ,IOptions<AwsSettings> options)
        {
            _amazonS3Client = amazonS3Client;
            _awsSettings = options.Value;
        }
        public async Task<string> UploadFileAsync(S3Object s3obj)
        {
            string key = "";
            try
            {
                var credentials = new BasicAWSCredentials(_awsSettings.UserCredentials.AccessKey, _awsSettings.UserCredentials.SecretKey);
                var secretClient = new Amazon.SecretsManager.AmazonSecretsManagerClient(credentials, RegionEndpoint.APSoutheast1);
                var s3SecretKeyBucket = await secretClient.GetSecretValueAsync(new Amazon.SecretsManager.Model.GetSecretValueRequest
                {
                    SecretId = "S3",
                    VersionStage = "AWSCURRENT"
                });
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = s3obj.InputStream,
                    Key = s3obj.Name,
                    BucketName = s3SecretKeyBucket.SecretString,
                    CannedACL = S3CannedACL.NoACL
                };

                var transferUtility = new TransferUtility(_amazonS3Client);

                await transferUtility.UploadAsync(uploadRequest);
                key = s3obj.Name;
                return key;
            }
            catch (AmazonS3Exception aex)
            {
                return key;
            }
            catch (Exception ex)
            {
                return key;
            }
        }

        public string GetPresignedUrl(string bucketName, string key, DateTime expiry)
        {
            var request = new Amazon.S3.Model.GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = expiry
            };
            string url = _amazonS3Client.GetPreSignedURL(request);
            return url;
        }
    }
}
