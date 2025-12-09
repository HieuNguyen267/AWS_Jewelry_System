using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Jewelry_Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Jewelry_API
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomRoleAuthorizationAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public CustomRoleAuthorizationAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<JewelryAwsContext>();

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            string? sub = null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            }
            catch
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (string.IsNullOrEmpty(sub))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var subGuid = Guid.Parse(sub);

            var demo = dbContext.Accounts.ToList();
            // Query DB để lấy user và role
            var user = await dbContext.Accounts
                    .FirstOrDefaultAsync(a => a.Id == subGuid);

            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check role
            if (!_roles.Contains(user.Role))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Gắn ClaimsPrincipal cho action nếu cần
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("role", user.Role)
        };
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "SystemIdentity"));

            await next();
        }
    }

}
