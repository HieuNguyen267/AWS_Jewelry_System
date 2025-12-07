using Amazon.Runtime;
using Amazon;
using Amazon.S3.Model;
using Jewelry_Model.Entity;
using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Product;
using Jewelry_Model.Payload.Response.Product;
using Jewelry_Model.Payload.Response.ProductSize;
using Jewelry_Model.Settings;
using Jewelry_Repository.Interface;
using Jewelry_Service.AwsS3.Models;
using Jewelry_Service.AwsS3.Services;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jewelry_Service.Implements;

public class ProductService : BaseService<ProductService>, IProductService
{
    private readonly IStorageService _storageService;

    private readonly CredentialsSetting _credentialsSetting;
    private readonly AwsSettings _awsSettings;
    private readonly S3Settings _s3Settings;
    public ProductService(IUnitOfWork<JewelryAwsContext> unitOfWork,
        ILogger<ProductService> logger, IHttpContextAccessor httpContextAccessor,
        IStorageService storageService, 
        IOptions<AwsSettings> options) 
        : base(unitOfWork, logger, httpContextAccessor)
    {
        _storageService = storageService;
        _awsSettings = options.Value;
        _s3Settings = options.Value.S3;
        _credentialsSetting = options.Value.UserCredentials;
    }

    public async Task<BaseResponse<CreateProductResponse>> CreateProduct(CreateProductRequest request)
    {
        string imageStr = "";
        //upload file to aws
        if (request.Image != null)
        {
            var credentials = new BasicAWSCredentials(_awsSettings.UserCredentials.AccessKey, _awsSettings.UserCredentials.SecretKey);
            var secretClient = new Amazon.SecretsManager.AmazonSecretsManagerClient(credentials, RegionEndpoint.APSoutheast1);
            var s3SecretKeyBucket = await secretClient.GetSecretValueAsync(new Amazon.SecretsManager.Model.GetSecretValueRequest
            {
                SecretId = "S3",
                VersionStage = "AWSCURRENT"
            });

            using var memoryStr = new MemoryStream();
            await request.Image.CopyToAsync(memoryStr);

            var fileExt = Path.GetExtension(request.Image.FileName);
            var objName = $"{Guid.NewGuid().ToString()}{fileExt}";

            var s3Obj = new AwsS3.Models.S3Object
            {
                BucketName = s3SecretKeyBucket.SecretString,
                InputStream = memoryStr,
                Name = objName 
            };

            var keyOfImage = await _storageService.UploadFileAsync(s3Obj);
            imageStr = $"https://{s3SecretKeyBucket.SecretString}.s3-ap-southeast-1.amazonaws.com/{keyOfImage}";
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Image = imageStr,
            IsNew = true,
            IsActive = true,
            CreateAt = DateTime.UtcNow
        };

        await _unitOfWork.GetRepository<Product>().InsertAsync(product);

        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;

        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình tạo sản phẩm");
        }

        return new BaseResponse<CreateProductResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Tạo sản phẩm thành công",
            Data = new CreateProductResponse()
            {
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
            }
        };
    }

    public async Task<BaseResponse<IPaginate<GetProductResponse>>> GetAllProduct(int page, int size)
    {
        var products = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
            selector: p => new GetProductResponse()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                Price = p.ProductSizes.Where(ps => ps.Price.HasValue && ps.IsActive == true).Select(ps => ps.Price.Value).Min(),
                //Quantity = p.ProductSizes.Where(ps => ps.Quantity.HasValue && ps.IsActive == true).Select(ps => ps.Quantity.Value).Sum(),
                Rating = p.Reviews.Average(r => (double?)r.Rating) ?? 0,
                Sizes = p.ProductSizes.ToList()

            },
            predicate: p => p.IsActive == true,
            include: p => p.Include(p => p.ProductSizes).Include(p => p.Reviews),
            page: page,
            size: size);

        return new BaseResponse<IPaginate<GetProductResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách sản phẩm thành công",
            Data = products
        };
    }

    public async Task<BaseResponse<GetProductDetailResponse>> GetProductById(Guid id)
    {
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            selector: p => new GetProductDetailResponse()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                productSizes = p.ProductSizes.
                    Where(ps => ps.IsActive == true).
                    Select(ps => new GetProductSizeResponse()
                    {
                        Id = ps.Id,
                        Price = ps.Price,
                        Quantity = ps.Quantity,
                        Size = ps.Size.Label
                    }).ToList(),
            },
            predicate: p => p.Id.Equals(id) && p.IsActive == true,
            include: p => p.Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
            );

        if (product == null)
        {
            new BaseResponse<GetProductDetailResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            };
        }

        return new BaseResponse<GetProductDetailResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy thông tin sản phẩm thành công",
            Data = product
        };
    }

    public async Task<BaseResponse<UpdateProductResponse>> UpdateProduct(Guid id, UpdateProductRequest request)
    {
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: p => p.Id.Equals(id) && p.IsActive == true);

        if (product == null)
        {
            return new BaseResponse<UpdateProductResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            };
        }

        string? uploadedImage = null;
        if (request.Image != null)
        {
            using var memoryStr = new MemoryStream();
            await request.Image.CopyToAsync(memoryStr);

            var fileExt = Path.GetExtension(request.Image.FileName);
            var objName = $"{Guid.NewGuid().ToString()}{fileExt}";
            var s3Obj = new AwsS3.Models.S3Object
            {
                BucketName = _s3Settings.BucketName,
                InputStream = memoryStr,
                Name = objName
            };
            uploadedImage = await _storageService.UploadFileAsync(s3Obj);
        }

        product.Name = request.Name ?? product.Name;
        product.Description = request.Description ?? product.Description;
        product.Image = uploadedImage ?? product.Image;

        _unitOfWork.GetRepository<Product>().UpdateAsync(product);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;

        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình cập nhật sản phẩm");
        }

        return new BaseResponse<UpdateProductResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật sản phẩm thành công",
            Data = new UpdateProductResponse()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
            }
        };
    }

    public async Task<BaseResponse<bool>> DeleteProduct(Guid id)
    {
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: p => p.Id.Equals(id) && p.IsActive == true);

        if (product == null)
        {
            return new BaseResponse<bool>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy sản phẩm",
                Data = false
            };
        }

        product.IsActive = false;
        product.DeleteAt = DateTime.UtcNow;
        _unitOfWork.GetRepository<Product>().UpdateAsync(product);

        var productSizes = await _unitOfWork.GetRepository<ProductSize>().GetListAsync(
            predicate: ps => ps.ProductId.Equals(id) && ps.IsActive == true);

        foreach (var productSize in productSizes)
        {
            productSize.IsActive = false;
        }
        _unitOfWork.GetRepository<ProductSize>().UpdateRange(productSizes);

        var isSuccess = await _unitOfWork.CommitAsync() > 0;

        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình xóa sản phẩm");
        }

        return new BaseResponse<bool>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Xóa sản phẩm thành công",
            Data = true
        };
    }
    
}