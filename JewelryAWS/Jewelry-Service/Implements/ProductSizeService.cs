using Jewelry_Model.Entity;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.ProductSize;
using Jewelry_Model.Payload.Response.ProductSize;
using Jewelry_Repository.Interface;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jewelry_Service.Implements;

public class ProductSizeService : BaseService<ProductSizeService>, IProductSizeService
{
    public ProductSizeService(IUnitOfWork<JewelryAwsContext> unitOfWork, ILogger<ProductSizeService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }

    public async Task<BaseResponse<bool>> DeleteProductSize(Guid id)
    {
        var productSize = await _unitOfWork.GetRepository<ProductSize>().SingleOrDefaultAsync(
            predicate: ps => ps.Id.Equals(id) && ps.IsActive == true);

        if (productSize == null)
        {
            return new BaseResponse<bool>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy thông tin chi tiết kích thước của sản phẩm",
                Data = false,
            };
        }
        
        productSize.IsActive = false;
        
        _unitOfWork.GetRepository<ProductSize>().UpdateAsync(productSize);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình xóa kích thước sản phẩm");
        }

        return new BaseResponse<bool>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Xóa kích thước của sản phẩm thành công",
            Data = true,
        };
    }

    public async Task<BaseResponse<GetProductSizeResponse>> UpdateProductSize(Guid id, UpdateProductSizeRequest request)
    {
        var productSize = await _unitOfWork.GetRepository<ProductSize>().SingleOrDefaultAsync(
            predicate: ps => ps.Id.Equals(id) && ps.IsActive == true,
            include: ps => ps.Include(ps => ps.Size));

        if (productSize == null)
        {
            return new BaseResponse<GetProductSizeResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy thông tin chi tiết kích thước của sản phẩm",
                Data = null,
            };
        }
        
        productSize.Quantity = request.Quantity ?? productSize.Quantity;
        productSize.Price = request.Price ?? productSize.Price;
        
        _unitOfWork.GetRepository<ProductSize>().UpdateAsync(productSize);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình cập nhật kích thước sản phẩm");
        }

        return new BaseResponse<GetProductSizeResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật kích thước sản phẩm thành công",
            Data = new GetProductSizeResponse()
            {
                Id = productSize.Id,
                Quantity = productSize.Quantity,
                Price = productSize.Price,
                Size = productSize.Size.Label
            }
        };
    }
}