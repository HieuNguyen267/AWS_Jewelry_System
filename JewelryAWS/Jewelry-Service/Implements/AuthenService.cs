using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Authentication;
using Jewelry_Model.Payload.Response.Authentication;
using Jewelry_Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Jewelry_Model.Entity;
using Jewelry_Model.Paginate;
using Jewelry_Model.Payload.Response.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Jewelry_Model.Settings;
using Jewelry_Repository.Interface;
using Microsoft.Extensions.Logging;
using Jewelry_Model.Utils;
using System.IdentityModel.Tokens.Jwt;
using Jewelry_Model.Enum;
using Jewelry_Model.Payload.Response.Account;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;

namespace Jewelry_Service.Implements
{
    /*
     {"id_token":"eyJraWQiOiJwM1lpamNvV0NkbkJzSjNEck1ZWHJUWGpHWWY3NFJcL1M5SXo4ekNDUFBxZz0iLCJhbGciOiJSUzI1NiJ9.eyJhdF9oYXNoIjoiQ1l2YThpUGtnXzdJTlVxWmM0SDlCUSIsInN1YiI6ImY5NmExNTRjLWYwNzEtNzA4YS0yM2NiLThjN2MxYTRkZWEwNSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtc291dGhlYXN0LTEuYW1hem9uYXdzLmNvbVwvYXAtc291dGhlYXN0LTFfdUNSbUxvb3hoIiwiY29nbml0bzp1c2VybmFtZSI6ImY5NmExNTRjLWYwNzEtNzA4YS0yM2NiLThjN2MxYTRkZWEwNSIsIm9yaWdpbl9qdGkiOiIzODc1NTFjYy02MGY4LTQ4MjYtYjhlNS1kOGZkYzQ3OGQ0NzkiLCJhdWQiOiIyZHUyNTRvbDlyMWZsMDQ0dHN1Y2hzaTIwbSIsInRva2VuX3VzZSI6ImlkIiwiYXV0aF90aW1lIjoxNzY1MDQ3MjE1LCJleHAiOjE3NjUwNTA4MTUsImlhdCI6MTc2NTA0NzIxNSwianRpIjoiZTJjNTRjZWMtMzU1ZS00NWU1LTk3NzItZjYzZjcwOWIxMzI4IiwiZW1haWwiOiJzZTE4NTA0N25ndXllbmR1eWhpZXVAZ21haWwuY29tIn0.f61tmLXGryRZduIwpwHtWG-Ct1kEY7i9uYvSJ1H3k3c8a3n3LfB1TSn8I2Wn1TI_QLVwdjnybS5PF3m3SRCKdfDdkjOAM96GqwKxXtJkWtsfQoEkO5NqdNfySg2D7n8d7EzzFURl8ktaK7n59BRF_JB0tBtagNU7w2jGar_M6n-oQ-DrA3ieCnDIr1692gn29YDWFpN3J6uNgRp9ZWFWdUNaJ5E7z6Q3bLOOgtRS3IThsXLdlwXHdLFIuH_PJv5PMoKrgUBdNX3Gxj3sJax1QMW7nos1B-I4ZhfdwKUZKDVmwOElIo8DoE90azaD1MoIrvY02khz8cZy1vZG7kpmvA","access_token":"eyJraWQiOiJQeFROcXBVRTM0RnVWdUI5SjV2VGpXUlNUM3Y4XC9hTGNhMENqdTlcL3BTYmM9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJmOTZhMTU0Yy1mMDcxLTcwOGEtMjNjYi04YzdjMWE0ZGVhMDUiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtc291dGhlYXN0LTEuYW1hem9uYXdzLmNvbVwvYXAtc291dGhlYXN0LTFfdUNSbUxvb3hoIiwidmVyc2lvbiI6MiwiY2xpZW50X2lkIjoiMmR1MjU0b2w5cjFmbDA0NHRzdWNoc2kyMG0iLCJvcmlnaW5fanRpIjoiMzg3NTUxY2MtNjBmOC00ODI2LWI4ZTUtZDhmZGM0NzhkNDc5IiwidG9rZW5fdXNlIjoiYWNjZXNzIiwic2NvcGUiOiJwaG9uZSBvcGVuaWQgZW1haWwiLCJhdXRoX3RpbWUiOjE3NjUwNDcyMTUsImV4cCI6MTc2NTA1MDgxNSwiaWF0IjoxNzY1MDQ3MjE1LCJqdGkiOiJkZGQ2ZGQ1OS03NWI0LTQ1M2QtYjUyYi02MjBmYjEzOWYzZDciLCJ1c2VybmFtZSI6ImY5NmExNTRjLWYwNzEtNzA4YS0yM2NiLThjN2MxYTRkZWEwNSJ9.JnKEv3HjYzCFEylbCvdg8wWdQYz10BZ-8WPuFjH1TxYcr1CEOO2HpS1O6ikpkG0e1-EkUmWO3wRE0m0BiX43thDsDOtUTVm1oF876Iae8oi6lrDelSoMMhoJXfYrwncMQq5wUe_kV4EyttTJa3IE2iR3t_noBB-dXWy57osOatlcK9NZbS7CO5uhXZYxLi1B-u6esiJ4j-MjyZgAzxdr5rN8n7vFCqhUsoIXSEUxJh2sJKRTcV8Bs4o-MkY7QVeRQwzxxtNHlIEW0WBeZLMTiRIGecA3uvN4UrLoXuJgticmEr4jpQ7apfwwnwoiQtOctdTODqZBnhBJndZnNfAiZQ","refresh_token":"eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.IcXGKDaMLjO8hr9zPIHgZXujvogsRFpvlTl9IyHog8MMUmllLAGwE8fNI0c3PkDTUaxsT8pJQJv7zmxgCNEXQB6_gr8P_eGTY5Y_31pbUWtHT1rydOPMvY98z32W2-0WP7G70g5XXZd-WMWtNQT_iWKaXYs1VMB4a2hX1RL9Bi0QU_wrr_iFcZvI2Jbyp0ANXgjcp6BoA5wQGNU0XlSSTxELQWaLTCDRIGVTyaQfFRUjFPirvQA-NJEMvngs30NEvS95UJ8Q0GaWCFXweY-wy81dpYtdfKc94GkYMBGqAgITumNSPYYaFAGos4D6x-hJT6CmG1gr5ORDAcuHS0ZFcg.uX2lps-sUid2tz1n.AgHnHZjhzbs3e_W3h_VnrzV59cVwd3X6v0XSbezyN9zlUK3ShXCn5dF9bksALGdJnyJJ_TorbEVPT6zxZoH_bfJCZhEHomtHlRApjc-0m4V2IPYwrefc0XUogduybiOc6vusgGlXyI6TzAs68_B0K64mb3G4wpKr1DXfh0i5BKeYeH-W12nMPIg6Cy7ITwnpoMunfXTgY0xnPqgr9SE4-mgHaDalUkiK7QHE68X_N0NM5XepgDgCkaVqUozpQH9u6UVDTDlwywN7PAYbms7MCqeIisycDz95Xqy1l095fytvmv-gix1Vt26--RKYwi7YnIwWr201FN7e3zwHCgbpeQMENSvN07CPx6EUHcy-tggpGzpgTzBzDlmKPCPJRGzZTJ2j2UcoPwjSyuThVkbumvD_H0Hn8P9Eubg78tL-Npxs3fRbbmjI0ZixUUwnW1VPzsWfnUFUXpf1T6UHtXmU608i6q1aPX5JLS6fctyS_gcjmyRvY_91RQkTHHGJaP15cuNjTuuwOVr9wYAz8h99Lamedmn78jqW6ZeTXVR1hNS60Uzjtad9Z5a9-ay6TOYmkU2RtgPnvtyXih8U6HLJ4NnrsD5dPogJhH-8GQ64rr09FAGs7Ce7WpvPzvMPvFzOy_0rkErvk3eUuy4t7pj2ObhWzirzoSfiaj9DvMMY3zxTmtyKkA6Kb-NILRTjNWWBybv3CvptOakmHHZkz-2P8-YVSgw9NnNsMTLj0KJ41vVfJaJzojkhSoljwKDfJ_gQ97fzC60cnow-TV40pwXjt6E690jGgy1nzShBEGX7W-mYd3Fs_cqGsN2qzduv0yoW16s7Kjo9uvFrrGg7thhPo7lYtMx1VXnwSAC6iQWI7diBl2i8Ftdfp9LSLFDhA7GQ1l2LxnvzK6X3UjcWuF4dmDrFRYes7_lSTPiNWQsdjrlxbKEt84-sDq-HfWUvk0tBn4eiae340ndvw9tcPHvIHn7CtQhxtjS5aNpEnDnQ_FCcFLxX_NeFwypExbnTXzQEcm3apYQvCx6K583cmuhmKJtbnBuxt14CNFopILAedIbQqRLE2fbVuTla34Rx1wVQBkJHz_L8tFWOExJtC1j9a-MMOeodVATyGRyUWU-W1DwWsyc7oUOKlSGoCErWf7pgB-CqXj0BYu_WmG5LF2h9eip96JGgkkIgPCwLQd5NDMbRBLRZDizFwp2Z9eES6ngvm6MauHNo4k6xeEQKOqDgQE1cjb50ry0006R622g85hgz5DJlbS4Ki34U.Vnt7AUISPGnVSTY9z1b3Gw","expires_in":3600,"token_type":"Bearer"}
     */
    internal class CognitoTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string Access_Token { get; set; }
        [JsonPropertyName("expires_in")]
        public int Expires_In { get; set; }
        [JsonPropertyName("token_type")]
        public string Token_Type { get; set; }
        [JsonPropertyName("refresh_token")]
        public string Refresh_Token { get; set; }
        [JsonPropertyName("id_token")]
        public string Id_Token { get; set; }
    }

    internal class CognitoUserInfoResponse
    {
        public string Sub { get; set; }
        public string Email_Verified { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class AuthenService : BaseService<AuthenService>, IAuthenService
    {
        private readonly HttpClient _http;

        private readonly CognitoSetting _cognito;
        public AuthenService(IUnitOfWork<JewelryAwsContext> unitOfWork, ILogger<AuthenService> logger,
            IHttpContextAccessor httpContextAccessor, HttpClient http, IOptions<AwsSettings> options) : base(unitOfWork, logger, httpContextAccessor)
        {
            _http = http;
            _cognito = options.Value.Cognito;
        }
        public async Task<BaseResponse<TokenExchangeResponse>> ExchangeTokenAsync(TokenExchangeRequest tokenExchangeRequest)
        {
            if (!_cognito.ClientId.Equals(tokenExchangeRequest.ClientId))
            {
                return new BaseResponse<TokenExchangeResponse>()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Xác thực thất bại",
                    Data = null
                };
            }

            var request = new HttpRequestMessage(HttpMethod.Post, _cognito.Domain + "/oauth2/token");

            // Body as x-www-form-urlencoded
            var form = new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", tokenExchangeRequest.Code),
                new KeyValuePair<string, string>("client_id", _cognito.ClientId),
                new KeyValuePair<string, string>("client_secret", _cognito.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", _cognito.ReturnUrl),
            };
            request.Content = new FormUrlEncodedContent(form);

            var response = await _http.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Một lỗi đã xảy ra trong quá trình xác thực với AWS");
            }

            var tokenResponse = JsonSerializer.Deserialize<CognitoTokenResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenResponse == null)
                throw new Exception("Một lỗi đã xảy ra trong quá trình xác thực");

            return new BaseResponse<TokenExchangeResponse>()
            {
                Status = StatusCodes.Status200OK,
                Message = "Xác thực thành công",
                Data = new TokenExchangeResponse
                {
                    AccessToken = tokenResponse.Access_Token,
                    RefreshToken = tokenResponse.Refresh_Token,
                    IdToken = tokenResponse.Id_Token
                }
            };
        }

        public async Task<BaseResponse<GetAccountResponse>> GetUserInfoAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _cognito.Domain + "/oauth2/userinfo");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _http.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Một lỗi đã xảy ra trong quá trình lấy thông tin người dùng với AWS");
            }

            var result = JsonSerializer.Deserialize<CognitoUserInfoResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var existedAccount = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(Guid.Parse(result.Sub!)) && a.IsActive == true);

            if (existedAccount == null)
            {
                //create user
                var account = new Account()
                {
                    Id = Guid.Parse(result.Sub!),
                    Email = result.Email,
                    Phone = "",
                    Password = "******",
                    Role = RoleEnum.User.GetDescriptionFromEnum(),
                    FullName = result.Username,
                    Address = "",
                    IsActive = true,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<Account>().InsertAsync(account);

                await _unitOfWork.CommitAsync();
            }

            var accountRes = new GetAccountResponse
            {

                Id = Guid.Parse(result!.Sub),
                Email = result.Email,
                FullName = result.Username
            };

            return new BaseResponse<GetAccountResponse>()
            {
                Status = StatusCodes.Status200OK,
                Message = "lấy thông tin người dùng thành công",
                Data = accountRes
            };
        }

        
    }
}
