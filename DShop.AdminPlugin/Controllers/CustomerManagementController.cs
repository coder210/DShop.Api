using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 客户管理
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class CustomerManagementController : ControllerBase
    {
        private readonly ICustomerQueryService _queryService;
        private readonly ICustomerCommandService _commandService;

        public CustomerManagementController(ICustomerQueryService queryService, ICustomerCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        /// <summary>
        /// 客户分页列表
        /// </summary>
        [SwaggerOperation(Summary = "客户列表", Description = "获取客户分页列表")]
        [AuthorizePermission("customer-management:list", "获取客户列表")]
        [HttpGet("GetList")]
        public IActionResult GetList([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _queryService.GetCustomerList(keyword, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 客户详情
        /// </summary>
        [SwaggerOperation(Summary = "客户详情", Description = "获取客户详情")]
        [AuthorizePermission("customer-management:get", "获取客户详情")]
        [HttpGet("Get/{id}")]
        public IActionResult Get(long id)
        {
            var result = _queryService.GetCustomerDetail(id, out string msg);
            return Ok(new ApiResponse { Code = result != null ? 200 : 404, Data = result, Msg = msg });
        }

        /// <summary>
        /// 更新客户状态
        /// </summary>
        [SwaggerOperation(Summary = "更新客户状态", Description = "启用/禁用客户")]
        [AuthorizePermission("customer-management:status", "更新客户状态")]
        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateCustomerStatusRequest request)
        {
            var (success, message) = _commandService.UpdateCustomerStatus(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 新增客户
        /// </summary>
        [SwaggerOperation(Summary = "新增客户", Description = "新建客户账号")]
        [AuthorizePermission("customer-management:create", "新增客户")]
        [HttpPost("Create")]
        public IActionResult Create([FromBody] CreateOrUpdateCustomerRequest request)
        {
            var (success, message) = _commandService.CreateCustomer(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 更新客户
        /// </summary>
        [SwaggerOperation(Summary = "更新客户", Description = "更新客户信息")]
        [AuthorizePermission("customer-management:update", "更新客户")]
        [HttpPost("Update")]
        public IActionResult Update([FromBody] CreateOrUpdateCustomerRequest request)
        {
            var (success, message) = _commandService.UpdateCustomer(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 客户收货地址
        /// </summary>
        [SwaggerOperation(Summary = "客户收货地址", Description = "获取客户收货地址列表")]
        [AuthorizePermission("customer-management:addresses", "查看客户收货地址")]
        [HttpGet("Addresses/{customerId}")]
        public IActionResult GetAddresses(long customerId)
        {
            var result = _queryService.GetCustomerAddresses(customerId);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 客户积分流水
        /// </summary>
        [SwaggerOperation(Summary = "客户积分流水", Description = "获取客户积分流水")]
        [AuthorizePermission("customer-management:coin", "查看客户积分流水")]
        [HttpGet("CoinRecords/{customerId}")]
        public IActionResult GetCoinRecords(long customerId, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _queryService.GetCoinRecords(customerId, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 客户浏览记录
        /// </summary>
        [SwaggerOperation(Summary = "客户浏览记录", Description = "获取客户浏览记录")]
        [AuthorizePermission("customer-management:browsing", "查看客户浏览记录")]
        [HttpGet("Browsing/{customerId}")]
        public IActionResult GetBrowsing(long customerId, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _queryService.GetBrowsingSpus(customerId, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 客户收藏列表
        /// </summary>
        [SwaggerOperation(Summary = "客户收藏", Description = "获取客户收藏列表")]
        [AuthorizePermission("customer-management:collect", "查看客户收藏")]
        [HttpGet("Collects/{customerId}")]
        public IActionResult GetCollects(long customerId, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _queryService.GetCollectSpus(customerId, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }
    }
}
