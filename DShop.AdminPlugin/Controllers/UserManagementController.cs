using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.PluginShared;
using DShop.AdminPlugin.Responses;
using DShop.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 用户管理页 - 用户CRUD、权限/菜单绑定、权限列表查询
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IIdentityQueryService _identityQueryService;
        private readonly IIdentityCommandService _identityCommandService;

        public UserManagementController(IIdentityQueryService identityQueryService, IIdentityCommandService identityCommandService)
        {
            _identityQueryService = identityQueryService;
            _identityCommandService = identityCommandService;
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "获取所有用户列表", Description = "获取所有用户列表")]
        [AuthorizePermission("user-management:get:list", "获取所有用户列表")]
        public IActionResult GetList()
        {
            var userList = _identityQueryService.GetUserList();
            var userResponses = new List<UserResponse>();
            foreach (var item in userList)
            {
                userResponses.Add(new UserResponse()
                {
                    Id = item.Id,
                    Avatar = item.Avatar,
                    Email = item.Email,
                    Sex = item.Sex,
                    Username = item.Username
                });
            }
            return Ok(new ApiResponse { Code = 200, Data = userResponses, Msg = "获取成功" });
        }

        /// <summary>
        /// 根据id获取用户信息
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "获取用户信息", Description = "根据id获取用户信息")]
        [AuthorizePermission("user-management:get", "根据id获取用户信息")]
        public IActionResult Get(long id)
        {
            if (_identityQueryService.GetUser(id, out User user, out string msg))
            {
                var userResponse = new UserResponse()
                {
                    Avatar = user.Avatar,
                    Email = user.Email,
                    Sex = user.Sex,
                    Username = user.Username
                };
                return Ok(new ApiResponse { Code = 200, Data = userResponse, Msg = "获取成功" });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = msg });
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        [SwaggerOperation(Summary = "创建用户", Description = "创建用户")]
        [AuthorizePermission("user-management:create", "创建用户")]
        [HttpPost("Create")]
        public IActionResult Create([FromForm] string userName, [FromForm] string password)
        {
            if (_identityCommandService.Register(userName, password, out string msg))
            {
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "创建成功" });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "创建失败:" + msg });
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        [SwaggerOperation(Summary = "修改用户信息", Description = "修改用户信息")]
        [AuthorizePermission("user-management:update", "修改用户信息")]
        [HttpPost("Update/{id}")]
        public IActionResult Update(long id, [FromForm] string? avatar, [FromForm] string? sex, [FromForm] string? email)
        {
            var userRequest = new UpdateUserRequest()
            {
                AvatarData = avatar ?? string.Empty,
                Sex = sex ?? string.Empty,
                Email = email ?? string.Empty,
            };
            if (_identityCommandService.UpdateUser(id, userRequest, out string msg))
            {
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = msg });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "修改失败:" + msg });
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [AuthorizePermission("user-management:delete", "根据id删除用户")]
        [HttpPost("Delete/{id}")]
        [SwaggerOperation(Summary = "删除用户", Description = "根据id删除用户")]
        public IActionResult Delete(long id)
        {
            if (_identityCommandService.DeleteUser(id, out string msg))
            {
                return Ok(new ApiResponse { Code = 200, Data = "", Msg = "删除成功" });
            }
            return Ok(new ApiResponse { Code = 400, Msg = "删除失败:" + msg });
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        [SwaggerOperation(Summary = "修改用户密码", Description = "重置/修改指定用户的登录密码")]
        [AuthorizePermission("user-management:change-password", "修改用户密码")]
        [HttpPost("ChangePassword/{id}")]
        public IActionResult ChangePassword(long id, [FromForm] string newPassword, [FromForm] string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "新密码不能为空" });
            }
            if (newPassword.Length < 6)
            {
                return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "密码长度至少为6位" });
            }
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "请再次输入确认密码" });
            }
            if (newPassword != confirmPassword)
            {
                return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "两次输入的密码不一致" });
            }
            if (_identityCommandService.UpdatePassword(id, newPassword, string.Empty, out string msg))
            {
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "密码修改成功" });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "密码修改失败:" + msg });
        }

        /// <summary>
        /// 获取用户的授权菜单id列表
        /// </summary>
        [SwaggerOperation(Summary = "用户的授权菜单id(列表)", Description = "获取用户的授权菜单id(列表)")]
        [AuthorizePermission("user-management:binding:menus", "获取用户的授权菜单id(列表)")]
        [HttpGet("{userId}/Menus")]
        public IActionResult GetBindingMenusList(long userId)
        {
            // 只回显用户直接绑定的菜单(UserMenus)，不含角色菜单。
            // 用户管理"绑定菜单"语义 = 只管理用户自己的额外菜单。
            var menus = _identityQueryService.GetUserDirectMenus(userId);
            var menuIdList = menus.Select(it => it.Id).ToList();
            return Ok(new ApiResponse { Code = 200, Data = menuIdList, Msg = "获取成功" });
        }

        /// <summary>
        /// 用户可见菜单（树形，含来源标注：direct/role/both）
        /// </summary>
        [SwaggerOperation(Summary = "用户可见菜单", Description = "获取用户所有能看到的菜单树，并标注来源")]
        [AuthorizePermission("user-management:visible-menus", "查看用户可见菜单")]
        [HttpGet("{userId}/VisibleMenus")]
        public IActionResult GetVisibleMenus(long userId)
        {
            var tree = _identityQueryService.GetUserVisibleMenus(userId);
            return Ok(new ApiResponse { Code = 200, Data = tree, Msg = "获取成功" });
        }

        /// <summary>
        /// 菜单树（供用户分配菜单）
        /// </summary>
        [HttpGet("Menus/Tree")]
        [SwaggerOperation(Summary = "菜单树", Description = "获取菜单树")]
        [AuthorizePermission("user-management:binding:menus", "获取菜单树")]
        public IActionResult GetMenuTree()
        {
            var tree = _identityQueryService.GetMenuTree(null);
            return Ok(new ApiResponse { Code = 200, Data = tree, Msg = "获取成功" });
        }

        /// <summary>
        /// 用户绑定菜单(多个)
        /// </summary>
        [SwaggerOperation(Summary = "用户绑定菜单", Description = "用户绑定菜单,多项menuIds以逗号分隔")]
        [AuthorizePermission("user-management:menus:bind", "用户绑定菜单")]
        [HttpPost("{userId}/Menus/Bind")]
        public IActionResult BindMenus(long userId, [FromForm] string? menuIds)
        {
            // 允许不勾选（空 menuIds）以清空该用户的直接菜单绑定
            var menuIdList = string.IsNullOrWhiteSpace(menuIds)
                ? new List<long>()
                : menuIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(id => Convert.ToInt64(id))
                         .ToList();
            if (_identityCommandService.BindMenuList(userId, menuIdList))
            {
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "绑定成功" });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "绑定失败" });
        }

        /// <summary>
        /// 获取用户绑定权限id列表
        /// </summary>
        [SwaggerOperation(Summary = "获取用户绑定权限id列表", Description = "获取用户绑定权限id列表")]
        [AuthorizePermission("user-management:binding:permissions", "获取用户绑定权限id列表")]
        [HttpGet("{userId}/Permissions")]
        public IActionResult GetBindingPermissionsList(long userId)
        {
            var permissions = _identityQueryService.GetUserPermissions(userId);
            var permissionIdList = permissions.Select(it => it.Id).ToList();
            return Ok(new ApiResponse { Code = 200, Data = permissionIdList, Msg = "获取成功" });
        }

        /// <summary>
        /// 用户绑定权限(多个)
        /// </summary>
        [SwaggerOperation(Summary = "用户绑定权限", Description = "用户绑定权限,多项permissionIds以逗号分隔")]
        [AuthorizePermission("user-management:permissions:bind", "用户绑定权限")]
        [HttpPost("{userId}/Permissions/Bind")]
        public IActionResult BindPermissions(long userId, [FromForm] string permissionIds)
        {
            var permissionIdList = permissionIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(id => Convert.ToInt64(id))
                                      .ToList();
            var (success, message) = _identityCommandService.BindPermissionList(userId, permissionIdList);
            if (success)
            {
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = message });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = message });
        }

        /// <summary>
        /// 获取所有权限列表（用于绑定）
        /// </summary>
        [SwaggerOperation(Summary = "获取所有权限列表", Description = "获取所有权限列表")]
        [AuthorizePermission("user-management:permissions:list", "获取所有权限列表")]
        [HttpGet("Permissions")]
        public IActionResult GetAllPermissions()
        {
            var permissionResponses = new List<Responses.PermissionResponse>();
            var permissions = _identityQueryService.GetPermissions();
            foreach (var item in permissions)
            {
                permissionResponses.Add(new Responses.PermissionResponse()
                {
                    Id = item.Id,
                    CreatedAt = item.CreatedAt,
                    Description = item.Description,
                    PermissionCode = item.PermissionCode,
                    Remark = item.Remark,
                    SortOrder = item.SortOrder,
                    UpdatedAt = item.UpdatedAt,
                });
            }
            return Ok(new ApiResponse { Code = 200, Data = permissionResponses, Msg = "获取成功" });
        }

        /// <summary>
        /// 获取所有权限列表（含模块分组信息，供前端按模块分组）
        /// </summary>
        [HttpGet("Permissions/All")]
        [AuthorizePermission("user-management:permissions:list", "获取所有权限列表")]
        public IActionResult GetAllPermissionsWithModule()
        {
            var permissions = _identityQueryService.GetPermissions();
            var result = permissions.Select(p => new
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
            return Ok(new ApiResponse { Code = 200, Data = result, Msg = "获取成功" });
        }

        /// <summary>
        /// 获取所有角色列表（用于绑定）
        /// </summary>
        [SwaggerOperation(Summary = "获取所有角色列表", Description = "获取所有角色列表")]
        [AuthorizePermission("user-management:roles:list", "获取所有角色列表")]
        [HttpGet("Roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _identityQueryService.GetRoles();
            var roleResponses = roles.Select(r => new
            {
                id = r.Id,
                code = r.Code,
                name = r.Name,
                description = r.Description,
            }).ToList();
            return Ok(new ApiResponse { Code = 200, Data = roleResponses, Msg = "获取成功" });
        }

        /// <summary>
        /// 获取用户已绑定角色id列表
        /// </summary>
        [SwaggerOperation(Summary = "获取用户已绑定角色id列表", Description = "获取用户已绑定角色id列表")]
        [AuthorizePermission("user-management:binding:roles", "获取用户已绑定角色id列表")]
        [HttpGet("{userId}/Roles")]
        public IActionResult GetBindingRolesList(long userId)
        {
            var roleIdList = _identityQueryService.GetUserRoleIds(userId);
            return Ok(new ApiResponse { Code = 200, Data = roleIdList, Msg = "获取成功" });
        }

        /// <summary>
        /// 用户绑定角色(多个，覆盖式)
        /// </summary>
        [SwaggerOperation(Summary = "用户绑定角色", Description = "用户绑定角色,多项roleIds以逗号分隔")]
        [AuthorizePermission("user-management:roles:bind", "用户绑定角色")]
        [HttpPost("{userId}/Roles/Bind")]
        public IActionResult BindRoles(long userId, [FromForm] string roleIds)
        {
            var roleIdList = roleIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(id => Convert.ToInt64(id))
                                   .ToList();
            if (roleIdList.Count == 0)
            {
                return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "请至少选择一个角色" });
            }
            var (success, message) = _identityCommandService.BindRoleList(userId, roleIdList);
            if (success)
            {
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = message });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = message });
        }
    }
}
