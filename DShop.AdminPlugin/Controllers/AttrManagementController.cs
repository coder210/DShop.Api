using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 属性管理
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AttrManagementController : ControllerBase
    {
        private readonly IProductQueryService _queryService;
        private readonly IProductCommandService _commandService;

        public AttrManagementController(IProductQueryService queryService, IProductCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        /// <summary>
        /// 属性库列表
        /// </summary>
        [SwaggerOperation(Summary = "属性列表", Description = "获取属性库列表（可选分类/属性类型筛选）")]
        [AuthorizePermission("attr-management:list", "获取属性列表")]
        [HttpGet("GetList")]
        public IActionResult GetList([FromQuery] long? categoryId, [FromQuery] int attrType = -1)
        {
            var result = _queryService.GetAttrList(categoryId, attrType);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 新建/更新属性
        /// </summary>
        [SwaggerOperation(Summary = "保存属性", Description = "新建或更新属性库属性")]
        [AuthorizePermission("attr-management:save", "保存属性")]
        [HttpPost("Save")]
        public IActionResult Save([FromBody] CreateOrUpdateAttrRequest request)
        {
            var (success, message) = _commandService.SaveAttr(request);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        [SwaggerOperation(Summary = "删除属性", Description = "删除属性库属性")]
        [AuthorizePermission("attr-management:delete", "删除属性")]
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(long id)
        {
            var (success, message) = _commandService.DeleteAttr(id);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = message });
        }
    }
}
