using Jewelry_API.Constant;
using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Product;
using Jewelry_Model.Payload.Request.ProductSize;
using Jewelry_Model.Payload.Request.Review;
using Jewelry_Model.Payload.Response.Product;
using Jewelry_Model.Payload.Response.ProductSize;
using Jewelry_Model.Payload.Response.Review;
using Jewelry_Model.Settings;
using Jewelry_Service.Implements;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Jewelry_API.Controller;

public class ProductController : BaseController<ProductController>
{
    private readonly IProductService _productService;
    private readonly IProductSizeService _productSizeService;
    private readonly IReviewService _reviewService;
    public ProductController(ILogger<ProductController> logger, IProductService productService, IReviewService reviewService, IProductSizeService productSizeService) : base(logger)
    {
        _productService = productService;
        _reviewService = reviewService;
        _productSizeService = productSizeService;
    }

    [HttpPost(ApiEndPointConstant.Product.CreateProduct)]
    [ProducesResponseType(typeof(BaseResponse<CreateProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateProductResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<CreateProductResponse>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
    {
        var response = await _productService.CreateProduct(request);
        return StatusCode(response.Status, response);
    }

    [HttpGet(ApiEndPointConstant.Product.GetPreSignedImage)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetPresignedProductImage(Guid productId)
    {
        var response = await _productService.GetProductImage(productId);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Product.GetAllProduct)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetProductResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> GetAllProduct([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var response = await _productService.GetAllProduct(page, size);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Product.GetProductById)]
    [ProducesResponseType(typeof(BaseResponse<GetProductDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetProductDetailResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        var response = await _productService.GetProductById(id);
        return StatusCode(response.Status, response);
    }
    
    [HttpPut(ApiEndPointConstant.Product.UpdateProduct)]
    [ProducesResponseType(typeof(BaseResponse<UpdateProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<UpdateProductResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromForm] UpdateProductRequest request)
    {
        var response = await _productService.UpdateProduct(id, request);
        return StatusCode(response.Status, response);
    }
    
    [HttpDelete(ApiEndPointConstant.Product.DeleteProduct)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var response = await _productService.DeleteProduct(id);
        return StatusCode(response.Status, response);
    }
    
    [HttpPost(ApiEndPointConstant.Product.CreateReview)]
    [ProducesResponseType(typeof(BaseResponse<CreateReviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateReviewResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateReview([FromRoute] Guid id, [FromBody] CreateReviewRequest request)
    {
        var response = await _reviewService.CreateReview(id, request);
        return StatusCode(response.Status, response);
    }

    [HttpGet(ApiEndPointConstant.Product.GetAllReview)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetReviewResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetReviewResponse>>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllReview([FromRoute] Guid id, [FromQuery] int? page, [FromQuery] int? size)
    {
        int pageNumber = page ?? 1;
        int pageSize = size ?? 10;
        var response = await _reviewService.GetAllReviews(id, pageNumber, pageSize);
        return StatusCode(response.Status, response);
    }

    //productSize
    [HttpGet(ApiEndPointConstant.ProductSize.GetProductSizes)]
    [ProducesResponseType(typeof(BaseResponse<List<GetProductSizeResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllProductSizes(Guid productId)
    {
        var response = await _productSizeService.GetSizesByProductId(productId);
        return StatusCode(response.Status, response);
    }

    [HttpPost(ApiEndPointConstant.ProductSize.CreateProductSize)]
    [ProducesResponseType(typeof(BaseResponse<GetProductSizeResponse>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateProductSizes(Guid productId, CreateProductSizeRequest data)
    {
        var response = await _productSizeService.CreateProductSizes(productId, data);
        return StatusCode(response.Status, response);
    }

    [HttpDelete(ApiEndPointConstant.ProductSize.DeleteProductSize)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteProductSize([FromRoute] Guid id)
    {
        var response = await _productSizeService.DeleteProductSize(id);
        return StatusCode(response.Status, response);
    }

    [HttpPut(ApiEndPointConstant.ProductSize.UpdateProductSize)]
    [ProducesResponseType(typeof(BaseResponse<GetProductSizeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetProductSizeResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateProductSize([FromRoute] Guid id, [FromBody] UpdateProductSizeRequest request)
    {
        var response = await _productSizeService.UpdateProductSize(id, request);
        return StatusCode(response.Status, response);
    }
}