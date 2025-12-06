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

namespace Jewelry_Service.Implements
{
    internal class CognitoTokenResponse
    {
        public string Access_Token { get; set; }
        public string Expires_In { get; set; }
        public string Token_Type { get; set; }
        public string Refresh_Token { get; set; }
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
                new KeyValuePair<string, string>("redirect_uri", tokenExchangeRequest.RedirectUri),
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
            var request = new HttpRequestMessage(HttpMethod.Get, _cognito + "/oauth2/userinfo");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _http.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                
            }else throw new Exception($"Một lỗi đã xảy ra trong quá trình lấy thông tin người dùng với AWS");

            var result = JsonSerializer.Deserialize<CognitoUserInfoResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var existedAccount = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(result.Sub) && a.IsActive == true);

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
