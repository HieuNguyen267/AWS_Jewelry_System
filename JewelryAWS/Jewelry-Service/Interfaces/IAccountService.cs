using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Account;
using Jewelry_Model.Payload.Response.Account;

namespace Jewelry_Service.Interfaces;

public interface IAccountService
{
    Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request);
    
    Task<BaseResponse<IPaginate<GetAccountResponse>>> GetAllAccounts(int page, int size);
    
    Task<BaseResponse<GetAccountResponse>> GetAccountById(Guid id);
    
    Task<BaseResponse<bool>> DeleteAccountById(Guid id);

    Task<BaseResponse<GetAccountResponse>> UpdateAccount(UpdateAccountRequest request);
}