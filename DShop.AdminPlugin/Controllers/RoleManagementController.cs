using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.PluginShared;
using DShop.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 角色管理页
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class RoleManagementController : ControllerBase
    {
        private readonly IIdentityQueryService _identityQueryService;
        private readonly IIdentityCommandService _identityCommandService;

        public RoleManagementController(IIdentityQueryService identityQueryService, IIdentityCommandService identityCommandService)
        {
            _identityQueryService = identityQueryService;
            _identityCommandService = identityCommandService;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        [SwaggerOperation(Summary = "获取角色列表", Description = "获取全部角色")]
        [AuthorizePermission("role-management:list", "获取角色列表")]
        [HttpGet("List")]
        public IActionResult GetList()
        {
            var result = _identityQueryService.GetRoles();
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        [SwaggerOperation(Summary = "创建角色", Description = "新增一个角色")]
        [AuthorizePermission("role-management:create", "创建角色")]
        [HttpPost("Create")]
        public IActionResult Create([FromBody] Role role)
        {
            var newId = _identityCommandService.CreateRole(role, out string msg);
            if (newId <= 0)
                return Ok(new ApiResponse { Code = 400, Data = null, Msg = msg });
            return Ok(new ApiResponse { Code = 200, Data = newId, Msg = msg });
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        [SwaggerOperation(Summary = "更新角色", Description = "修改角色信息")]
        [AuthorizePermission("role-management:update", "更新角色")]
        [HttpPost("Update")]
        public IActionResult Update([FromBody] Role role)
        {
            var success = _identityCommandService.UpdateRole(role, out string msg);
            if (!success)
                return Ok(new ApiResponse { Code = 400, Data = null, Msg = msg });
            return Ok(new ApiResponse { Code = 200, Data = null, Msg = msg });
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [SwaggerOperation(Summary = "删除角色", Description = "删除指定角色")]
        [AuthorizePermission("role-management:delete", "删除角色")]
        [HttpPost("Delete")]
        public IActionResult Delete([FromQuery] int id)
        {
            var success = _identityCommandService.DeleteRole(id, out string msg);
            if (!success)
                return Ok(new ApiResponse { Code = 400, Data = null, Msg = msg });
            return Ok(new ApiResponse { Code = 200, Data = null, Msg = msg });
        }

        /// <summary>
        /// 绑定用户角色
        /// </summary>
        [SwaggerOperation(Summary = "绑定用户角色", Description = "为用户分配角色")]
        [AuthorizePermission("role-management:bind-user", "绑定用户角色")]
        [HttpPost("BindUserRoles")]
        public IActionResult BindUserRoles([FromQuery] long userId, [FromBody] List<int> roleIds)
        {
            _identityCommandService.BindRoleList(userId, roleIds);
            return Ok(new ApiResponse { Code = 200, Data = null, Msg = "绑定成功" });
        }

        /// <summary>
        /// 绑定角色菜单
        /// </summary>
        [SwaggerOperation(Summary = "绑定角色菜单", Description = "为角色分配菜单")]
        [AuthorizePermission("role-management:bind-menu", "绑定角色菜单")]
        [HttpPost("BindRoleMenus")]
        public IActionResult BindRoleMenus([FromQuery] int roleId, [FromBody] List<long> menuIds)
        {
            var success = _identityCommandService.BindRoleMenus(roleId, menuIds);
            if (!success)
                return Ok(new ApiResponse { Code = 400, Data = null, Msg = "绑定失败" });
            return Ok(new ApiResponse { Code = 200, Data = null, Msg = "绑定成功" });
        }

        /// <summary>
        /// 绑定角色权限
        /// </summary>
        [SwaggerOperation(Summary = "绑定角色权限", Description = "为角色分配操作权限")]
        [AuthorizePermission("role-management:bind-permission", "绑定角色权限")]
        [HttpPost("BindRolePermissions")]
        public IActionResult BindRolePermissions([FromQuery] int roleId, [FromBody] List<long> permissionIds)
        {
            var success = _identityCommandService.BindRolePermissions(roleId, permissionIds);
            if (!success)
                return Ok(new ApiResponse { Code = 400, Data = null, Msg = "绑定失败" });
            return Ok(new ApiResponse { Code = 200, Data = null, Msg = "绑定成功" });
        }

        /// <summary>
        /// 获取角色菜单
        /// </summary>
        [SwaggerOperation(Summary = "获取角色菜单", Description = "获取角色拥有的菜单")]
        [AuthorizePermission("role-management:list", "获取角色列表")]
        [HttpGet("RoleMenus")]
        public IActionResult GetRoleMenus([FromQuery] int roleId)
        {
            var result = _identityQueryService.GetRoleMenus(roleId);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        [SwaggerOperation(Summary = "获取角色权限", Description = "获取角色拥有的操作权限")]
        [AuthorizePermission("role-management:list", "获取角色列表")]
        [HttpGet("RolePermissions")]
        public IActionResult GetRolePermissions([FromQuery] int roleId)
        {
            var result = _identityQueryService.GetRolePermissions(roleId);
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        [SwaggerOperation(Summary = "获取用户角色", Description = "获取用户拥有的角色")]
        [AuthorizePermission("role-management:list", "获取角色列表")]
        [HttpGet("UserRoles")]
        public IActionResult GetUserRoles([FromQuery] long userId)
        {
            var roleIds = _identityQueryService.GetUserRoleIds(userId);
            return Ok(new ApiResponse { Code = 200, Data = roleIds, Msg = "获取成功" });
        }
    }
}
