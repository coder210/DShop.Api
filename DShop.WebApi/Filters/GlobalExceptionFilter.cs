using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Api.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Log.Error(context.Exception, "发生未处理异常: {Message}", context.Exception.Message);

            var response = new 
            {
                Code = 500,
                Msg = "服务器开小差了~~"
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true; // 标记异常已处理
        }
    }
}
