using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 售后/退款管理
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class RefundManagementController : ControllerBase
    {
        private readonly IRefundQueryService _queryService;
        private readonly IRefundCommandService _commandService;

        public RefundManagementController(IRefundQueryService queryService, IRefundCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        /// <summary>
        /// 售后/退款分页列表
        /// </summary>
        [SwaggerOperation(Summary = "退款列表", Description = "获取售后/退款分页列表")]
        [AuthorizePermission("refund-management:list", "获取退款列表")]
        [HttpGet("GetList")]
        public IActionResult GetList([FromQuery] string? keyword, [FromQuery] int status = -1, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _queryService.GetRefundList(keyword, status, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 售后/退款详情
        /// </summary>
        [SwaggerOperation(Summary = "退款详情", Description = "获取售后/退款详情")]
        [AuthorizePermission("refund-management:get", "获取退款详情")]
        [HttpGet("Get/{id}")]
        public IActionResult Get(long id)
        {
            var result = _queryService.GetRefundDetail(id, out string msg);
            return Ok(new ApiResponse { Code = result != null ? 200 : 404, Data = result, Msg = msg });
        }

        /// <summary>
        /// 审核退款
        /// </summary>
        [SwaggerOperation(Summary = "审核退款", Description = "同意或拒绝退款申请")]
        [AuthorizePermission("refund-management:audit", "审核退款")]
        [HttpPost("Audit")]
        public IActionResult Audit([FromBody] AuditRefundRequest request)
        {
            var (success, message) = _commandService.AuditRefund(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }
    }
}
