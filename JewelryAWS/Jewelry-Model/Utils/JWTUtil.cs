using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewelry_Model.Utils
{
    public static class JWTUtil
    {
        public static Dictionary<string, string> ReadClaims(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            return token.Claims
                .ToDictionary(x => x.Type, x => x.Value);
        }

        public static string? GetClaim(string accessToken, string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            return token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}
