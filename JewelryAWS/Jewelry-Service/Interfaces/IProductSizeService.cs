using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.ProductSize;
using Jewelry_Model.Payload.Response.ProductSize;

namespace Jewelry_Service.Interfaces;

public interface IProductSizeService
{
    Task<BaseResponse<bool>> DeleteProductSize(Guid id);
    
    Task<BaseResponse<GetProductSizeResponse>> UpdateProductSize(Guid id, UpdateProductSizeRequest request); 
}