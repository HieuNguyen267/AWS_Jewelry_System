using Jewelry_API.Constant;
using Jewelry_Model.Paginate;
using Jewelry_Model.Payload;
using Jewelry_Model.Payload.Request.Account;
using Jewelry_Model.Payload.Response.Account;
using Jewelry_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_API.Controller;

public class AccountController : BaseController<AccountController>
{
    private readonly IAccountService _accountService;
    public AccountController(ILogger<AccountController> logger, IAccountService accountService) : base(logger)
    {
     
        _accountService = accountService;
    }

    [CustomRoleAuthorization("Admin")]
    [HttpPost(ApiEndPointConstant.Account.RegisterAccount)]
    [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> RegisterAccount([FromBody] RegisterRequest request)
    {
        _logger.LogInformation($"Register account called for email: {request.Email}");
        var response = await _accountService.Register(request);
        return StatusCode(response.Status, response);
    }

    [CustomRoleAuthorization("Admin")]
    [HttpGet(ApiEndPointConstant.Account.GetAccounts)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetAccountResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccounts([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        _logger.LogInformation($"Get accounts called. page: {page}, size: {size}");
        var response = await _accountService.GetAllAccounts(page, size);
        return StatusCode(response.Status, response);
    }

    [CustomRoleAuthorization("Admin")]
    [HttpGet(ApiEndPointConstant.Account.GetAccount)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccount([FromRoute] Guid id)
    {
        _logger.LogInformation($"Get account called for id: {id}");
        var response = await _accountService.GetAccountById(id);
        return StatusCode(response.Status, response);
    }

    [CustomRoleAuthorization("Admin")]
    [HttpPut(ApiEndPointConstant.Account.UpdateAccount)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountRequest request)
    {
        _logger.LogInformation($"Update account called");
        var response = await _accountService.UpdateAccount(request);
        return StatusCode(response.Status, response);
    }

    [CustomRoleAuthorization("Admin")]
    [HttpDelete(ApiEndPointConstant.Account.DeleteAccount)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteAccount([FromRoute] Guid id)
    {
        _logger.LogInformation($"Delete account called for id: {id}");
        var response = await _accountService.DeleteAccountById(id);
        return StatusCode(response.Status, response);
    }
}