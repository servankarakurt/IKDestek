using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRSupport.UI.Filters
{
    public class RequireLoginFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller)
                && string.Equals(controller, "Auth", StringComparison.OrdinalIgnoreCase))
                return;

            var token = context.HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl = context.HttpContext.Request.Path });
        }
    }
}
