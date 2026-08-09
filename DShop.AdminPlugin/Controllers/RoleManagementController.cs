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
    /// 角色管理 - 角色CRUD、角色绑定菜单/权限
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
        /// 角色列表
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "角色列表", Description = "获取所有角色")]
        [AuthorizePermission("role-management:list", "获取角色列表")]
        public IActionResult GetList()
        {
            var roles = _identityQueryService.GetRoles();
            var data = roles.Select(r => new
            {
                id = r.Id,
                code = r.Code,
                name = r.Name,
                description = r.Description,
                sortOrder = r.SortOrder,
                isSystem = r.IsSystem
            }).ToList();
            return Ok(new ApiResponse { Code = 200, Data = data, Msg = "获取成功" });
        }

        /// <summary>
        /// 角色详情（含已绑定菜单/权限id）
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "角色详情", Description = "获取角色及其菜单/权限")]
        [AuthorizePermission("role-management:detail", "获取角色详情")]
        public IActionResult Get(int id)
        {
            var role = _identityQueryService.GetRoles().FirstOrDefault(r => r.Id == id);
            if (role == null)
                return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "角色不存在" });

            var data = new
            {
                id = role.Id,
                code = role.Code,
                name = role.Name,
                description = role.Description,
                sortOrder = role.SortOrder,
                isSystem = role.IsSystem,
                menuIds = _identityQueryService.GetRoleMenus(id),
                permissionIds = _identityQueryService.GetRolePermissions(id)
            };
            return Ok(new ApiResponse { Code = 200, Data = data, Msg = "获取成功" });
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        [HttpPost("Create")]
        [SwaggerOperation(Summary = "创建角色", Description = "创建角色")]
        [AuthorizePermission("role-management:create", "创建角色")]
        public IActionResult Create([FromForm] string code, [FromForm] string name,
            [FromForm] string? description, [FromForm] int sortOrder = 0)
        {
            var role = new Role
            {
                Code = code,
                Name = name,
                Description = description ?? string.Empty,
                SortOrder = sortOrder,
                IsSystem = false
            };
            var newId = _identityCommandService.CreateRole(role, out string msg);
            if (newId > 0)
                return Ok(new ApiResponse { Code = 200, Data = new { id = newId }, Msg = msg });
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = msg });
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        [HttpPost("Update/{id}")]
        [SwaggerOperation(Summary = "修改角色", Description = "修改角色")]
        [AuthorizePermission("role-management:update", "修改角色")]
        public IActionResult Update(int id, [FromForm] string code, [FromForm] string name,
            [FromForm] string? description, [FromForm] int sortOrder = 0)
        {
            var role = new Role
            {
                Id = id,
                Code = code,
                Name = name,
                Description = description ?? string.Empty,
                SortOrder = sortOrder,
                IsSystem = false
            };
            if (_identityCommandService.UpdateRole(role, out string msg))
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = msg });
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = msg });
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpPost("Delete/{id}")]
        [SwaggerOperation(Summary = "删除角色", Description = "删除角色")]
        [AuthorizePermission("role-management:delete", "删除角色")]
        public IActionResult Delete(int id)
        {
            if (_identityCommandService.DeleteRole(id, out string msg))
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = msg });
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = msg });
        }

        /// <summary>
        /// 角色已绑定菜单id列表
        /// </summary>
        [HttpGet("{id}/Menus")]
        [SwaggerOperation(Summary = "角色菜单", Description = "获取角色已绑定菜单id")]
        [AuthorizePermission("role-management:menus", "获取角色菜单")]
        public IActionResult GetMenus(int id)
        {
            var menuIds = _identityQueryService.GetRoleMenus(id);
            return Ok(new ApiResponse { Code = 200, Data = menuIds, Msg = "获取成功" });
        }

        /// <summary>
        /// 角色绑定菜单
        /// </summary>
        [HttpPost("{id}/Menus/Bind")]
        [SwaggerOperation(Summary = "角色绑定菜单", Description = "角色绑定菜单,多项menuIds以逗号分隔")]
        [AuthorizePermission("role-management:menus-bind", "角色绑定菜单")]
        public IActionResult BindMenus(int id, [FromForm] string? menuIds)
        {
            // 允许不勾选（空 menuIds）以清空该角色的菜单绑定
            var menuIdList = string.IsNullOrWhiteSpace(menuIds)
                ? new List<long>()
                : menuIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(m => Convert.ToInt64(m))
                         .ToList();
            if (_identityCommandService.BindRoleMenus(id, menuIdList))
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "绑定成功" });
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "绑定失败" });
        }

        /// <summary>
        /// 角色已绑定权限id列表
        /// </summary>
        [HttpGet("{id}/Permissions")]
        [SwaggerOperation(Summary = "角色权限", Description = "获取角色已绑定权限id")]
        [AuthorizePermission("role-management:permissions", "获取角色权限")]
        public IActionResult GetPermissions(int id)
        {
            var permissionIds = _identityQueryService.GetRolePermissions(id);
            return Ok(new ApiResponse { Code = 200, Data = permissionIds, Msg = "获取成功" });
        }

        /// <summary>
        /// 角色绑定权限
        /// </summary>
        [HttpPost("{id}/Permissions/Bind")]
        [SwaggerOperation(Summary = "角色绑定权限", Description = "角色绑定权限,多项permissionIds以逗号分隔")]
        [AuthorizePermission("role-management:permissions-bind", "角色绑定权限")]
        public IActionResult BindPermissions(int id, [FromForm] string permissionIds)
        {
            var permissionIdList = permissionIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(p => Convert.ToInt64(p))
                                    .ToList();
            if (_identityCommandService.BindRolePermissions(id, permissionIdList))
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "绑定成功" });
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "绑定失败" });
        }

        /// <summary>
        /// 菜单树（供角色分配菜单）
        /// </summary>
        [HttpGet("Menus/Tree")]
        [SwaggerOperation(Summary = "菜单树", Description = "获取菜单树")]
        [AuthorizePermission("role-management:menus-tree", "获取菜单树")]
        public IActionResult GetMenuTree()
        {
            var tree = _identityQueryService.GetMenuTree(null);
            return Ok(new ApiResponse { Code = 200, Data = tree, Msg = "获取成功" });
        }

        /// <summary>
        /// 所有权限（供角色分配权限）
        /// </summary>
        [HttpGet("Permissions/All")]
        [SwaggerOperation(Summary = "所有权限", Description = "获取所有权限")]
        [AuthorizePermission("role-management:permissions-all", "获取所有权限")]
        public IActionResult GetAllPermissions()
        {
            var permissions = _identityQueryService.GetPermissions()
                .Select(p => new
                {
                    id = p.Id,
                    permissionCode = p.PermissionCode,
                    description = p.Description,
                    module = p.Module,
                    endpoint = p.Endpoint,
                    apiPath = p.ApiPath,
                    remark = p.Remark,
                    sortOrder = p.SortOrder
                }).ToList();
            return Ok(new ApiResponse { Code = 200, Data = permissions, Msg = "获取成功" });
        }
    }
}
