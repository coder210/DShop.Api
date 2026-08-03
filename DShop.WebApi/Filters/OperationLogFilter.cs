using DShop.Infrastructure;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Diagnostics;
using System.Text;

namespace Api.Filters
{
    public class OperationLogFilter : IAsyncActionFilter
    {
        private readonly ILogger<OperationLogFilter> _logger;

        public OperationLogFilter(ILogger<OperationLogFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            var httpContext = context.HttpContext;

            // 获取用户信息（如果有）
            var userContext = httpContext.RequestServices.GetService<IUserContext>();
            var userId = userContext?.CurrentUserId;

            // 获取请求信息
            var controller = context.Controller.GetType().Name.Replace("Controller", "");
            var action = context.ActionDescriptor.RouteValues["action"];
            var method = httpContext.Request.Method;
            var url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
            var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();

            // 执行 Action
            var resultContext = await next();
            stopwatch.Stop();

            var statusCode = resultContext.HttpContext.Response.StatusCode;
            var exception = resultContext.Exception;

            // ★★★ 核心改动：使用 Serilog 写入文件（结构化日志）★★★
            var sb = new StringBuilder();
            sb.Append($"UserId: {userId}, ");
            sb.Append($"Controller: {controller}, ");
            sb.Append($"Action: {action}, ");
            sb.Append($"Method: {method}, ");
            sb.Append($"StatusCode: {statusCode}, ");
            sb.Append($"Elapsed: {stopwatch.ElapsedMilliseconds}ms, ");
            sb.Append($"ClientIp: {clientIp}, ");
            sb.Append($"Url: {url}");

            var message = sb.ToString();
            if (exception != null)
            {
                Log.Error(exception, message);
            }
            else
            {
                Log.Information(message);
            }

            if (exception != null) throw exception;
        }
    }
}
