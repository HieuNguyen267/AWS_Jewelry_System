using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Size;
using Jewelry_Model.Payload.Response.Size;

namespace Jewelry_Service.Interfaces;

public interface ISizeService
{
    Task<BaseResponse<CreateSizeResponse>> CreateSize(CreateSizeRequest request);

    Task<BaseResponse<IPaginate<GetSizeResponse>>> GetSizes(int page, int size);
    
    Task<BaseResponse<GetSizeResponse>> GetSize(Guid id);
}