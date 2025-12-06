namespace Jewelry_Model.Settings
{
    public class AwsSettings
    {
        public CredentialsSetting UserCredentials { get; set; }
        public S3Settings S3 { get; set; }
        public CognitoSetting Cognito { get; set; }
    }

    public class CognitoSetting
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;        // https://cognito-idp.<REGION>.amazonaws.com
        public string ReturnUrl { get; set; } = string.Empty;
    }
    public class CredentialsSetting
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
    public sealed class S3Settings
    {
        public string Region { get; init; } = string.Empty;
        public string BucketName { get; init; } = string.Empty;
    }
}
