
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Authentication;
using Jewelry_Model.Payload.Response.Account;
using Jewelry_Model.Payload.Response.Authentication;

namespace Jewelry_Service.Interfaces
{
    public interface IAuthenService
    {
        Task<BaseResponse<TokenExchangeResponse>> ExchangeTokenAsync(TokenExchangeRequest request);
        Task<BaseResponse<GetAccountResponse>> GetUserInfoAsync(string accessToken);
    }
}
