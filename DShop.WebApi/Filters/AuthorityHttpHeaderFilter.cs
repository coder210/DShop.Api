using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Filters
{
    public class AuthorityHttpHeaderFilter : IOperationFilter
    {
        //public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        //{
        //    if (operation.parameters == null)
        //        operation.parameters = new List<Parameter>();

        //    //判断是否添加权限过滤器

        //    var isAuthorized = apiDescription.ActionDescriptor.GetCustomAttributes<ApiAuthorizeAttribute>().Any();
        //    if (isAuthorized)
        //    {
        //        operation.parameters.Add(new Parameter { name = "token", @in = "header", description = "令牌", required = false, type = "string" });
        //    }
        //}
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //var isAuthorized = context.ApiDescription.ActionDescriptor.AttributeRouteInfo;
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "access_token",
                In = ParameterLocation.Header,
                Description = "令牌(Bearer )",
                Required = false,
                //Schema = new OpenApiSchema() { Type = "string" }
            });


        }
    }
}
