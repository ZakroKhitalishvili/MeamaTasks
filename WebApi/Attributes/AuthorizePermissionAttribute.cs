
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Meama_Tasks.Attributes;

public class AuthorizePermissionAttribute : ActionFilterAttribute
{
    public Permission AuthorizePermissions;
    public bool RequireRootRole;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role);
        var roleManager = context.HttpContext.RequestServices.GetService<RoleManager<AppRole>>();
        if (roleClaim is not null)
        {
            var role = await roleManager.FindByNameAsync(roleClaim.Value);

            if (RequireRootRole && role.IsRootRole == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //Checks if AuthorizePermissions are fully contained in a role
            if (role.IsRootRole || role.Permission.Contains(AuthorizePermissions))
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }
        }
        context.Result = new UnauthorizedResult();
    }
}