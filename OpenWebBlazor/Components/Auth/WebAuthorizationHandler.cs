using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace OpenWebBlazor.Components.Auth;

public class WebAuthorizationHandler : AuthorizationHandler<IAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

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
                // if (checkUrl(url))
                // {
                //     context.Succeed(requirement);
                // }
                // else
                // {
                //     context.Fail();
                // }
            }
        }
        else if (context.Resource is HttpContext httpContext)
        {
            var url = httpContext.Request.Path.Value;
            // if (checkUrl(url))
            // {
            //     context.Succeed(requirement);
            // }
            // else
            // {
            //     context.Fail();
            // }
        }

        return Task.CompletedTask;
    }
}