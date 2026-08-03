using DShop.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.BasePlugin.Controllers
{
    /// <summary>
    /// 健康检查 - 用于监控/网关探测插件状态
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// 检查 BasePlugin 是否正常运行
        /// </summary>
        [AllowAnonymous]
        [SwaggerOperation(Summary = "健康检查", Description = "检测 BasePlugin 插件的运行状态")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new ApiResponse
            {
                Code = 200,
                Data = new
                {
                    Plugin = "DShop.BasePlugin",
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow
                },
                Msg = "服务正常"
            });
        }
    }
}
