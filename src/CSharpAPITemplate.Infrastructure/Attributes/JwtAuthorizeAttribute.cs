using CSharpAPITemplate.Infrastructure.Results.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CSharpAPITemplate.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string[] Roles { get; set; }

        public JwtAuthorizeAttribute(params string[] roles)
        {
            Roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = (string?)context.HttpContext.Items["UserId"];
            var userRoles = (string?)context.HttpContext.Items["Roles"];

            // If UserId is null, then user is not logged in.
            if (userId == null)
            {
                var result = new BaseResult
                {
                    StatusCode = StatusCodes.Status401Unauthorized, 
                    Errors = new ErrorResult
                    {
                        Message = "Unauthorized",
                        Description = "The method you are accessing requires authorization"
                    }
                };
                context.Result = new JsonResult(result)
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                
                return;
            }

            // If there aren't any interceptions, then user is not authorized.
            // But if there are no role restrictions, then it's ok.
            if (string.IsNullOrEmpty(userRoles) || (!Roles.Intersect(userRoles.Split(",")).Any() && Roles.Any()))
            {
                var result = new BaseResult
                {
                    StatusCode = StatusCodes.Status403Forbidden, 
                    Errors = new ErrorResult
                    {
                        Message = "Forbidden",
                        Description = "You are not authorized to access this resource"
                    }
                };
                context.Result = new JsonResult(result)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}