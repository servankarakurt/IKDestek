using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace HRSupport.WebAPI.Swagger
{
    /// <summary>
    /// [AllowAnonymous] olan endpoint'lerde Swagger'da Bearer zorunluluğunu kaldırır (login gibi).
    /// </summary>
    public class SwaggerAllowAnonymousFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAllowAnonymous = context.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null
                || context.MethodInfo.DeclaringType?.GetCustomAttribute<AllowAnonymousAttribute>() != null;

            if (hasAllowAnonymous && operation.Security.Count > 0)
            {
                operation.Security = new List<OpenApiSecurityRequirement>();
            }
        }
    }
}
