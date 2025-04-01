using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using OpenWebBlazor.Services;

namespace OpenWebBlazor.Components.Auth;

public class WebAuthorizationHandler : AuthorizationHandler<IAuthorizationRequirement>
{
    private readonly RoleService _roleService;
    public WebAuthorizationHandler(RoleService roleService)
    {
        _roleService = roleService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }
        else
        {
            var user_id = 0;
            int.TryParse(context.User.FindFirstValue("UserId"), out user_id);

            if (context.Resource is Microsoft.AspNetCore.Components.RouteData routeData)
            {
                var routeAttr = routeData.PageType.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(RouteAttribute));
                if (routeAttr == null)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    var url = routeAttr.ConstructorArguments[0].Value as string;
                    var check_result = await _roleService.CheckAuth(user_id, url);
                    if (check_result.Success)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }
            else if (context.Resource is HttpContext httpContext)
            {
                var url = httpContext.Request.Path.Value;
                var check_result = await _roleService.CheckAuth(user_id, url);
                if (check_result.Success)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            return;
        }
    }
}