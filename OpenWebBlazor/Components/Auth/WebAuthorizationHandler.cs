using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;
using OpenWebBlazor.Services;
using OpenWebBlazor.ViewModels;

namespace OpenWebBlazor.Components.Auth;

public class WebAuthorizationHandler : AuthorizationHandler<IAuthorizationRequirement>
{
    private readonly IMemoryCache _cache;

    public WebAuthorizationHandler(IMemoryCache cache)
    {
        _cache = cache;
    }
    private bool CheckPathAuth(string user_id, string path)
    {
        var menus = _cache.Get<List<WebMenuTree>>(user_id);
        if (menus == null || !menus.Any())
            return false;

        if (menus.Any(a => a.Path == path))
        {
            return true;
        }

        return true;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }
        else
        {
            var user_id = context.User.FindFirstValue("UserId");

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
                    if (CheckPathAuth(user_id, url))
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
                if (CheckPathAuth(user_id, url))
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