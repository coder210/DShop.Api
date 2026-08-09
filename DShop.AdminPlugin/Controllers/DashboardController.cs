using DShop.Contracts;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 首页看板
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardQueryService _queryService;

        public DashboardController(IDashboardQueryService queryService)
        {
            _queryService = queryService;
        }

        /// <summary>
        /// 看板总览
        /// </summary>
        [SwaggerOperation(Summary = "首页看板", Description = "获取首页看板总览数据（订单/销售额/商品/客户/趋势）")]
        [AuthorizePermission("dashboard:overview", "获取首页看板数据")]
        [HttpGet("Overview")]
        public IActionResult Overview()
        {
            var result = _queryService.GetOverview();
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }
    }
}
