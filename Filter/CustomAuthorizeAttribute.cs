using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace hrms_api.Filter;

public class CustomAuthorizeAttribute : AuthorizeAttribute , IAuthorizationFilter
{
   private readonly string[] _roles;

   public CustomAuthorizeAttribute(params string[] roles)
   {
      _roles = roles;
   }

   public void OnAuthorization(AuthorizationFilterContext context)
   {
      var user = context.HttpContext.User;
      if (user.Identity != null && (!user.Identity.IsAuthenticated || !_roles.Any(roles => user.IsInRole(roles))))
      {
         context.Result = new ObjectResult( new
         {
            message = "You are not authorized to perform this action."
         })
         {
            StatusCode = StatusCodes.Status403Forbidden

         };
      }
   }
}