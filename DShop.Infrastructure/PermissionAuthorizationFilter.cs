using DShop.PluginShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DShop.Infrastructure
{
    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 1. 允许匿名访问
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
                return;

            // 2. 获取权限特性（先找方法，再找控制器）
            var permissionAttr = context.ActionDescriptor.EndpointMetadata
                .FirstOrDefault(em => em is AuthorizePermissionAttribute) as AuthorizePermissionAttribute;

            if (permissionAttr == null)
                return; // 没有权限要求，放行

            // 3. 用户必须已认证
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated == true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // 4. 拼接完整的权限码（端前缀来自特性声明的 Client，默认 admin；不再依赖命名空间推导）
            var client = permissionAttr.Client ?? "admin";
            var fullPermissionCode = $"{client}::{permissionAttr.PermissionCode}";

            // 5. 检查权限
            if (user.Claims.Any(c => c.Type == "permissions"))
            {
                var permissions = user.Claims.FirstOrDefault(c => c.Type == "permissions").Value;
                if (!permissions.Split(",").Contains(fullPermissionCode))
                {
                    context.Result = new ForbidResult();
                }
            }
            else
            {
                context.Result = new ForbidResult();
            }

        }
    }
}
