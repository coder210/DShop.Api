using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 分类管理
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class CategoryManagementController : ControllerBase
    {
        private readonly IProductQueryService _queryService;
        private readonly IProductCommandService _commandService;

        public CategoryManagementController(IProductQueryService queryService, IProductCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        /// <summary>
        /// 分类树
        /// </summary>
        [SwaggerOperation(Summary = "分类树", Description = "获取商品分类树")]
        [AuthorizePermission("category-management:list", "获取分类")]
        [HttpGet("GetTree")]
        public IActionResult GetTree()
        {
            var result = _queryService.GetCategoryTree();
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 新建/更新分类
        /// </summary>
        [SwaggerOperation(Summary = "保存分类", Description = "新建或更新商品分类")]
        [AuthorizePermission("category-management:save", "保存分类")]
        [HttpPost("Save")]
        public IActionResult Save([FromBody] CreateOrUpdateCategoryRequest request)
        {
            var (success, message) = _commandService.SaveCategory(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        [SwaggerOperation(Summary = "删除分类", Description = "删除商品分类")]
        [AuthorizePermission("category-management:delete", "删除分类")]
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(long id)
        {
            var (success, message) = _commandService.DeleteCategory(id);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }
    }
}
