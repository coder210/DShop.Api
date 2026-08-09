using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class TemplateManagementController : ControllerBase
    {
        private readonly ISystemQueryService _systemQueryService;
        private readonly ISystemCommandService _systemCommandService;

        public TemplateManagementController(ISystemQueryService systemQueryService, ISystemCommandService systemCommandService)
        {
            _systemQueryService = systemQueryService;
            _systemCommandService = systemCommandService;
        }

        [HttpGet("GetList")]
        [SwaggerOperation(Summary = "模板列表", Description = "获取模板列表")]
        [AuthorizePermission("template-management:list", "获取模板列表")]
        public IActionResult GetList([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _systemQueryService.GetTemplateList(keyword, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        [HttpGet("Get/{id}")]
        [SwaggerOperation(Summary = "模板详情", Description = "获取模板详情")]
        [AuthorizePermission("template-management:get", "获取模板详情")]
        public IActionResult Get(long id)
        {
            var result = _systemQueryService.GetTemplateById(id);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = result != null ? "获取成功" : "模板不存在" });
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "创建模板", Description = "创建新模板")]
        [AuthorizePermission("template-management:create", "创建模板")]
        public IActionResult Create([FromBody] CreateTemplateRequest request)
        {
            var success = _systemCommandService.CreateTemplate(request, out string msg);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = msg });
        }

        [HttpPost("Update")]
        [SwaggerOperation(Summary = "更新模板", Description = "更新模板信息")]
        [AuthorizePermission("template-management:update", "更新模板")]
        public IActionResult Update([FromBody] UpdateTemplateRequest request)
        {
            var success = _systemCommandService.UpdateTemplate(request, out string msg);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = msg });
        }

        [HttpPost("Delete/{id}")]
        [SwaggerOperation(Summary = "删除模板", Description = "删除模板")]
        [AuthorizePermission("template-management:delete", "删除模板")]
        public IActionResult Delete(long id)
        {
            var success = _systemCommandService.DeleteTemplate(id, out string msg);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = msg });
        }
    }
}
