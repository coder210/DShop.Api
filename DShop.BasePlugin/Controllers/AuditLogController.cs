using DShop.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DShop.BasePlugin.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly ISystemQueryService _systemQueryService;

        public AuditLogController(ISystemQueryService systemQueryService)
        {
            _systemQueryService = systemQueryService;
        }

        [HttpGet("GetList")]
        public IActionResult GetList(
            [FromQuery] string? keyword,
            [FromQuery] string? action,
            [FromQuery] string? tableName,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            try
            {
                var result = _systemQueryService.GetAuditLogList(keyword, action, tableName, dateFrom, dateTo, page, size);
                return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Code = 500, Msg = $"获取审计日志失败: {ex.Message}" });
            }
        }

        [HttpGet("GetDetail/{id}")]
        public IActionResult GetDetail(long id)
        {
            try
            {
                var result = _systemQueryService.GetAuditLogDetail(id, out string msg);
                if (result == null)
                    return Ok(new ApiResponse { Code = 404, Msg = msg });
                return Ok(new ApiResponse { Code = 200, Data = result, Msg = msg });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Code = 500, Msg = $"获取审计日志详情失败: {ex.Message}" });
            }
        }
    }
}
