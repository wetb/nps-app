using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace NPSApplication.API.Filters;

public class AuthorizeRoleAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public AuthorizeRoleAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userRole = context.HttpContext.User
            .FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole) || !_roles.Contains(userRole))
        {
            context.Result = new ForbidResult();
        }

        base.OnActionExecuting(context);
    }
}