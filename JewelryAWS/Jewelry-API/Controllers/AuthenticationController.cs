using Jewelry_API.Constant;
using Jewelry_Model.Payload;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Jewelry_Model.Payload.Response.Authentication;
using Jewelry_Model.Payload.Request.Authentication;
using Jewelry_Model.Payload.Response.Account;
using Microsoft.AspNetCore.Authorization;
using Jewelry_API.Controller;
using Amazon.Runtime.Internal;

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
        _logger.LogInformation($"Exchange token for code: {request.Code}");
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
        _logger.LogInformation($"Get user information");
        var response = await _authenService.GetUserInfoAsync(accessToken);
        return StatusCode(response.Status, response);
    }

    [CustomRoleAuthorization("Admin")]
    [HttpGet("/demo1")]
    public async Task<IActionResult> Demo1()
    {
        return Ok("Demo role cho admin");
    }

    [CustomRoleAuthorization("User")]
    [HttpGet("/demo2")]
    public async Task<IActionResult> Demo2()
    {
        return Ok("Demo role cho admin");
    }


}
