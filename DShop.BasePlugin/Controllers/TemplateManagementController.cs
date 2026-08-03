using DShop.Contracts;
using DShop.Contracts.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DShop.BasePlugin.Controllers
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
        public IActionResult GetList([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var result = _systemQueryService.GetTemplateList(keyword, page, size);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        [HttpGet("Get/{id}")]
        public IActionResult Get(long id)
        {
            var result = _systemQueryService.GetTemplateById(id);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = result != null ? "获取成功" : "模板不存在" });
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] CreateTemplateRequest request)
        {
            var success = _systemCommandService.CreateTemplate(request, out string msg);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = msg });
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] UpdateTemplateRequest request)
        {
            var success = _systemCommandService.UpdateTemplate(request, out string msg);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = msg });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(long id)
        {
            var success = _systemCommandService.DeleteTemplate(id, out string msg);
            return Ok(new ApiResponse { Code = success ? 200 : 400, Msg = msg });
        }
    }
}
