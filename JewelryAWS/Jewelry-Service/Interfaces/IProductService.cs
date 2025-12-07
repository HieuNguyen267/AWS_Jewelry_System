using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Product;
using Jewelry_Model.Payload.Response.Product;

namespace Jewelry_Service.Interfaces;

public interface IProductService
{
    Task<BaseResponse<CreateProductResponse>> CreateProduct(CreateProductRequest request);

    Task<BaseResponse<IPaginate<GetProductResponse>>> GetAllProduct(int page, int size);
    
    Task<BaseResponse<GetProductDetailResponse>> GetProductById(Guid id);
    
    Task<BaseResponse<UpdateProductResponse>> UpdateProduct(Guid id, UpdateProductRequest request);
    
    Task<BaseResponse<bool>> DeleteProduct(Guid id);
}