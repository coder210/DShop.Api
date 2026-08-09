using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 商品管理
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class ProductManagementController : ControllerBase
    {
        private readonly IProductQueryService _queryService;
        private readonly IProductCommandService _commandService;

        public ProductManagementController(IProductQueryService queryService, IProductCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        /// <summary>
        /// 商品SPU分页列表
        /// </summary>
        [SwaggerOperation(Summary = "商品列表", Description = "获取商品SPU分页列表")]
        [AuthorizePermission("product-management:list", "获取商品列表")]
        [HttpGet("GetList")]
        public IActionResult GetList([FromQuery] string? keyword, [FromQuery] long? categoryId, [FromQuery] int status = -1, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _queryService.GetSpuList(keyword, categoryId, status, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 商品SPU详情
        /// </summary>
        [SwaggerOperation(Summary = "商品详情", Description = "获取商品SPU详情（含SKU、图片、属性）")]
        [AuthorizePermission("product-management:get", "获取商品详情")]
        [HttpGet("Get/{id}")]
        public IActionResult Get(long id)
        {
            var result = _queryService.GetSpuDetail(id, out string msg);
            return Ok(new ApiResponse { Code = result != null ? 200 : 404, Data = result, Msg = msg });
        }

        /// <summary>
        /// 新建/更新商品
        /// </summary>
        [SwaggerOperation(Summary = "保存商品", Description = "新建或更新商品SPU（含SKU、图片、属性）")]
        [AuthorizePermission("product-management:save", "保存商品")]
        [HttpPost("Save")]
        public IActionResult Save([FromBody] CreateOrUpdateSpuRequest request)
        {
            var (success, message) = _commandService.SaveSpu(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 更新商品状态
        /// </summary>
        [SwaggerOperation(Summary = "上下架商品", Description = "更新商品SPU上下架状态")]
        [AuthorizePermission("product-management:status", "上下架商品")]
        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateSpuStatusRequest request)
        {
            var (success, message) = _commandService.UpdateSpuStatus(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        [SwaggerOperation(Summary = "删除商品", Description = "删除商品SPU")]
        [AuthorizePermission("product-management:delete", "删除商品")]
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(long id)
        {
            var (success, message) = _commandService.DeleteSpu(id);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }
    }
}
