using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 品牌管理
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class BrandManagementController : ControllerBase
    {
        private readonly IProductQueryService _queryService;
        private readonly IProductCommandService _commandService;

        public BrandManagementController(IProductQueryService queryService, IProductCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        /// <summary>
        /// 品牌分页列表
        /// </summary>
        [SwaggerOperation(Summary = "品牌列表", Description = "获取品牌分页列表")]
        [AuthorizePermission("brand-management:list", "获取品牌列表")]
        [HttpGet("GetList")]
        public IActionResult GetList([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _queryService.GetBrandList(keyword, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 新建/更新品牌
        /// </summary>
        [SwaggerOperation(Summary = "保存品牌", Description = "新建或更新品牌")]
        [AuthorizePermission("brand-management:save", "保存品牌")]
        [HttpPost("Save")]
        public IActionResult Save([FromBody] CreateOrUpdateBrandRequest request)
        {
            var (success, message) = _commandService.SaveBrand(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 删除品牌
        /// </summary>
        [SwaggerOperation(Summary = "删除品牌", Description = "删除品牌")]
        [AuthorizePermission("brand-management:delete", "删除品牌")]
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(long id)
        {
            var (success, message) = _commandService.DeleteBrand(id);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }
    }
}
