using Amazon.Runtime.Internal;
using Jewelry_Model.Entity;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.ProductSize;
using Jewelry_Model.Payload.Response.Product;
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

    #region product endpoints
    public async Task<BaseResponse<List<GetProductSizeResponse>>> GetSizesByProductId(Guid productId)
    {
        var existedProduct = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
           predicate: p => p.Id.Equals(productId) && p.IsActive == true);

        if (existedProduct == null)
        {
            return new BaseResponse<List<GetProductSizeResponse>>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Sản phẩm không tồn tại",
            };
        }

        var productSizes = _unitOfWork.GetRepository<ProductSize>().GetListAsync(
            predicate: pz => pz.ProductId.Equals(productId),
            include: q => q.Include(pz => pz.Size));
        return new BaseResponse<List<GetProductSizeResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy kích thước sản phẩm thành công",
            Data = productSizes.Result.Select(ps => new GetProductSizeResponse
            {
                Id = ps.Id,
                Size = ps.Size.Label,
                Price = ps.Price,
                Quantity = ps.Quantity,
                IsActive = ps.IsActive ?? false
            }).ToList()
        };
    }

    #endregion


    public async Task<BaseResponse<GetProductSizeResponse>> CreateProductSizes(Guid productId, CreateProductSizeRequest request)
    {

        //check product exist
        var existedProduct = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: p => p.Id.Equals(productId) && p.IsActive == true);

        if(existedProduct == null)
        {
            return new BaseResponse<GetProductSizeResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Sản phẩm không tồn tại",
            };
        }

        var existedSize = await _unitOfWork.GetRepository<Size>().SingleOrDefaultAsync(
            predicate: p => p.Id.Equals(request.SizeId) && p.IsActive == true);

        if (existedSize == null)
        {
            return new BaseResponse<GetProductSizeResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Nhãn kích thước không tồn tại",
            };
        }
        var productSize = new ProductSize
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            SizeId = request.SizeId,
            Price = request.Price,
            Quantity = request.Quantity,
            IsActive = true
        };

        await _unitOfWork.GetRepository<ProductSize>().InsertAsync(productSize);
        await _unitOfWork.CommitAsync();

        return new BaseResponse<GetProductSizeResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Tạo kích thước cho sản phẩm thành công",
            Data =  new GetProductSizeResponse
            {
                Id = productSize.Id,
                Size = existedSize.Label,
                Price = productSize.Price,
                Quantity = productSize.Quantity
            }
        };

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
       
        
        _unitOfWork.GetRepository<ProductSize>().DeleteAsync(productSize);
        
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
        request.IsActive ??= false;
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
        productSize.IsActive = request.IsActive;
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