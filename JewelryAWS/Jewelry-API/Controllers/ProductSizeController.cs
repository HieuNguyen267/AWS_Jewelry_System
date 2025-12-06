using Jewelry_API.Constant;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.ProductSize;
using Jewelry_Model.Payload.Response.ProductSize;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_API.Controller;

public class ProductSizeController : BaseController<ProductSizeController>
{
    private readonly IProductSizeService _productSizeService;

    public ProductSizeController(ILogger<ProductSizeController> logger, IProductSizeService productSizeService) : base(logger)
    {
        _productSizeService = productSizeService;
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