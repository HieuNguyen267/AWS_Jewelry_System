using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewelry_Model.Settings
{
    public class CognitoSetting
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;        // https://cognito-idp.<REGION>.amazonaws.com
        public string ReturnUrl { get; set; } = string.Empty;
    }
}
