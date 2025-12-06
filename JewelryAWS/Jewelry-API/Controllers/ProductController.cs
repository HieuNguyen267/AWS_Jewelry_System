using Jewelry_API.Constant;
using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Product;
using Jewelry_Model.Payload.Response.Product;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_API.Controller;

public class ProductController : BaseController<ProductController>
{
    private readonly IProductService _productService;
    public ProductController(ILogger<ProductController> logger, IProductService productService) : base(logger)
    {
        _productService = productService;
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
    
    [HttpGet(ApiEndPointConstant.Product.GetAllProduct)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetProductResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllProduct([FromQuery] int? page, [FromQuery] int? size)
    {
        int pageNumber = page ?? 1;
        int pageSize = size ?? 10;
        var response = await _productService.GetAllProduct(pageNumber, pageSize);
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
}