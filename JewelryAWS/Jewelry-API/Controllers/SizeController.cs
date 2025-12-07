using Amazon.Runtime.Internal;
using Jewelry_API.Constant;
using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Size;
using Jewelry_Model.Payload.Response.Size;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_API.Controller;

public class SizeController : BaseController<SizeController>
{
    private readonly ISizeService _sizeService;

    public SizeController(ILogger<SizeController> logger, ISizeService sizeService) : base(logger)
    {
        _sizeService = sizeService;
    }
    
    [HttpPost(ApiEndPointConstant.Size.CreateSize)]
    [ProducesResponseType(typeof(BaseResponse<CreateSizeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateSizeResponse>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateSize([FromBody] CreateSizeRequest request)
    {
        _logger.LogInformation($"Create size with label \"{request.Label}\"");
        var response = await _sizeService.CreateSize(request);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Size.GetSizes)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetSizeResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetSizes([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        _logger.LogInformation($"Get sizes called. page: {page}, size: {size}");
        var response = await _sizeService.GetSizes(page, size);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Size.GetSize)]
    [ProducesResponseType(typeof(BaseResponse<GetSizeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetSizeResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetSize([FromRoute] Guid id)
    {
        _logger.LogInformation($"Get size called with id: {id}");
        var response = await _sizeService.GetSize(id);
        return StatusCode(response.Status, response);
    }
}