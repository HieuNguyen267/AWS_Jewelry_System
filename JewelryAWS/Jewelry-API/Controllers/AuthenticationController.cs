using Jewelry_API.Constant;
using Jewelry_Model.Payload;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Jewelry_Model.Payload.Response.Authentication;
using Jewelry_Model.Payload.Request.Authentication;
using Jewelry_Model.Payload.Response.Account;
using Microsoft.AspNetCore.Authorization;
using Jewelry_API.Controller;

namespace Jewelry_API.Controllers;
public class AuthenticationController : BaseController<AccountController>
{
    private readonly IAuthenService _authenService;
    public AuthenticationController(ILogger<AccountController> logger, IAuthenService authenService) : base(logger)
    {
        _authenService = authenService;
    }

    [HttpPost(ApiEndPointConstant.Authentication.ExchangeToken)]
    [ProducesResponseType(typeof(BaseResponse<TokenExchangeResponse>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> ExchangeToken([FromBody] TokenExchangeRequest request)
    {
        var response = await _authenService.ExchangeTokenAsync(request);
        return StatusCode(response.Status, response);
    }

    [Authorize]
    [HttpGet(ApiEndPointConstant.Authentication.UserInfo)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> UserInfo()
    {
        var accessToken = HttpContext.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        var response = await _authenService.GetUserInfoAsync(accessToken);
        return StatusCode(response.Status, response);
    }

}
