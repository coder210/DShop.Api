using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 菜单管理页
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class MenuManagementController : ControllerBase
    {
        private readonly IIdentityQueryService _identityQueryService;
        private readonly IIdentityCommandService _identityCommandService;

        public MenuManagementController(IIdentityQueryService identityQueryService, IIdentityCommandService identityCommandService)
        {
            _identityQueryService = identityQueryService;
            _identityCommandService = identityCommandService;
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        [SwaggerOperation(Summary = "获取菜单树", Description = "获取所有菜单的树形结构")]
        [AuthorizePermission("menu-management:tree", "获取菜单树")]
        [HttpGet("Tree")]
        public IActionResult GetTree()
        {
            var result = _identityQueryService.GetMenuTree();
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        [SwaggerOperation(Summary = "创建菜单", Description = "创建新菜单")]
        [AuthorizePermission("menu-management:create", "创建菜单")]
        [HttpPost("Create")]
        public IActionResult Create([FromBody] MenuCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_identityCommandService.AddMenu(request))
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "创建成功" });
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "创建失败" });
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        [SwaggerOperation(Summary = "更新菜单", Description = "更新菜单信息")]
        [AuthorizePermission("menu-management:update", "更新菜单")]
        [HttpPost("Update")]
        public IActionResult Update([FromBody] MenuUpdateRequest request)
        {
            if (_identityCommandService.UpdateMenu(request, out string msg))
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "更新成功" });
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "更新失败:" + msg });
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        [SwaggerOperation(Summary = "删除菜单", Description = "删除指定菜单")]
        [AuthorizePermission("menu-management:delete", "删除菜单")]
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(long id)
        {
            if (_identityCommandService.DeleteMenu(id, out string msg))
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "删除成功" });
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "删除失败:" + msg });
        }
    }
}
