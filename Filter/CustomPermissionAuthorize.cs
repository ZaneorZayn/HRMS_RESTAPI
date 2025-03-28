using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace hrms_api.Filter;

public class CustomPermissionAuthorize : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permission;

    public CustomPermissionAuthorize(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new ObjectResult(new
                {
                    message = "You are not authenticated.",
                    status = StatusCodes.Status401Unauthorized,
                }
            )
            {
                StatusCode = StatusCodes.Status401Unauthorized,
            }; 
            return;
        }
        
        var hasPermission = user.Claims.Any(x => x.Type == "Permission" && x.Value == _permission);

        if (!hasPermission)
        {
            context.Result = new ObjectResult(new
            {
                message = "You do not have permission to access this resource.",
                status = StatusCodes.Status403Forbidden,
            })
            {
                StatusCode = StatusCodes.Status403Forbidden,
            };
        }
    }

}
