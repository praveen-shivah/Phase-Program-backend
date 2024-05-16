namespace IdentityServerAPI
{
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.OpenApi.Models;

    using Swashbuckle.AspNetCore.SwaggerGen;

    public class JwtTokenHeaderFilter : IOperationFilter
    {
        void IOperationFilter.Apply(
            OpenApiOperation operation,
            OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null || descriptor.ActionName == "Authenticate"  || descriptor.ActionName == "Logout" || descriptor.ActionName == "RefreshToken")
            {
                return;
            }

            operation.Parameters.Add(
                new OpenApiParameter
                    {
                        Name = "Access-Token",
                        In = ParameterLocation.Header,
                        Description = "JWT Token",
                        Required = true,
                        Schema = new OpenApiSchema { Type = "string" }
                    });
        }
    }
}