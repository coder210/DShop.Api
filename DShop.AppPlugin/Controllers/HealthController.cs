using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DShop.AppPlugin.Controllers;

/// <summary>
/// App 端健康检查占位控制器，用于验证插件路由 api/app 是否正常加载。
/// </summary>
[ApiController]
[Route("api/app/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get() => Ok(new { plugin = "AppPlugin", status = "ok" });
}
