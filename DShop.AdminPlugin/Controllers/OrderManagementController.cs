using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 订单管理
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class OrderManagementController : ControllerBase
    {
        private readonly IOrderQueryService _queryService;
        private readonly IOrderCommandService _commandService;

        public OrderManagementController(IOrderQueryService queryService, IOrderCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        /// <summary>
        /// 订单分页列表
        /// </summary>
        [SwaggerOperation(Summary = "订单列表", Description = "获取订单分页列表")]
        [AuthorizePermission("order-management:list", "获取订单列表")]
        [HttpGet("GetList")]
        public IActionResult GetList([FromQuery] string? keyword, [FromQuery] int status = -1, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _queryService.GetOrderList(keyword, status, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        [SwaggerOperation(Summary = "订单详情", Description = "获取订单详情（含明细、操作历史）")]
        [AuthorizePermission("order-management:get", "获取订单详情")]
        [HttpGet("Get/{id}")]
        public IActionResult Get(long id)
        {
            var result = _queryService.GetOrderDetail(id, out string msg);
            return Ok(new ApiResponse { Code = result != null ? 200 : 404, Data = result, Msg = msg });
        }

        /// <summary>
        /// 订单发货
        /// </summary>
        [SwaggerOperation(Summary = "订单发货", Description = "订单发货，填写物流信息")]
        [AuthorizePermission("order-management:ship", "订单发货")]
        [HttpPost("Ship")]
        public IActionResult Ship([FromBody] ShipOrderRequest request)
        {
            var (success, message) = _commandService.ShipOrder(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        [SwaggerOperation(Summary = "更新订单状态", Description = "更新订单状态（如关闭订单）")]
        [AuthorizePermission("order-management:status", "更新订单状态")]
        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateOrderStatusRequest request)
        {
            var (success, message) = _commandService.UpdateOrderStatus(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }
    }
}
