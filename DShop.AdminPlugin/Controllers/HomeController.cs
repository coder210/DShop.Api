using DShop.AdminPlugin.Responses;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Models;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace DShop.AdminPlugin.Controllers
{
    /// <summary>
    /// 首页/主布局 - 当前用户信息、菜单树、修改密码、退出、更新个人信息
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IIdentityQueryService _identityQueryService;
        private readonly IIdentityCommandService _identityCommandService;

        public HomeController(IIdentityQueryService identityQueryService, IIdentityCommandService identityCommandService)
        {
            _identityQueryService = identityQueryService;
            _identityCommandService = identityCommandService;
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        [SwaggerOperation(Summary = "获取当前用户信息", Description = "获取当前登录用户的信息")]
        [AuthorizePermission("home:get-user", "获取当前用户信息")]
        [HttpGet("GetUser")]
        public IActionResult GetUser()
        {
            var userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (_identityQueryService.GetUser(userId, out User user, out string msg))
            {
                var userViewModel = new UserResponse()
                {
                    Avatar = user.Avatar,
                    Email = user.Email,
                    Sex = user.Sex,
                    Username = user.Username
                };
                return Ok(new ApiResponse { Code = 200, Data = userViewModel, Msg = "获取成功" });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = msg });
        }

        /// <summary>
        /// 获取当前登录用户的授权菜单(树形)
        /// </summary>
        [SwaggerOperation(Summary = "获取当前用户菜单树", Description = "获取当前登录用户的授权菜单(树形)")]
        [AuthorizePermission("home:get-menus", "获取当前用户菜单树")]
        [HttpGet("GetMenus")]
        public IActionResult GetMenus()
        {
            var userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var menus = _identityQueryService.GetUserMenus(userId);

            // 临时补全可见菜单的祖先（仅用于构建导航树，不写回绑定表），
            // 保证勾选子菜单时左侧导航能向上展开到顶级菜单
            menus = _identityQueryService.ExpandMenuAncestors(menus);

            var topMenus = menus.Where(it => it.ParentId == 0).OrderBy(it => it.SortOrder).ToList();
            var userMenus = new List<UserMenuResponse>();

            foreach (var item in topMenus)
            {
                var menuResponse = BuildMenuTree(item, menus);
                userMenus.Add(menuResponse);
            }

            return Ok(new ApiResponse { Code = 200, Data = userMenus, Msg = "获取成功" });
        }

        private UserMenuResponse BuildMenuTree(Menu menu, List<Menu> allMenus)
        {
            var response = new UserMenuResponse
            {
                Id = menu.Id,
                Name = menu.Name,
                Icon = menu.Icon,
                Path = menu.Path,
                SortOrder = menu.SortOrder,
                Children = new List<UserMenuResponse>()
            };

            var children = allMenus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.SortOrder).ToList();
            foreach (var child in children)
            {
                response.Children.Add(BuildMenuTree(child, allMenus));
            }

            return response;
        }

        /// <summary>
        /// 修改密码(站内修改)
        /// </summary>
        [SwaggerOperation(Summary = "修改密码", Description = "站内修改密码")]
        [AuthorizePermission("home:update-password", "修改密码")]
        [HttpPost("UpdatePassword")]
        public IActionResult UpdatePassword([FromForm] string newPassword, [FromForm] string captcha)
        {
            var userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (_identityCommandService.UpdatePassword(userId, newPassword, captcha, out string msg))
            {
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = "修改成功" });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = msg });
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        [SwaggerOperation(Summary = "退出登录", Description = "退出登录，使 token 失效")]
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (_identityCommandService.Logout(userId, out string msg))
            {
                return Ok(new ApiResponse { Code = 200, Data = userId, Msg = "登出成功" });
            }
            return Ok(new ApiResponse { Code = 400, Data = userId, Msg = msg });
        }

        /// <summary>
        /// 修改当前登录用户个人信息
        /// </summary>
        [SwaggerOperation(Summary = "修改当前用户信息", Description = "修改当前登录用户的个人信息")]
        [AuthorizePermission("home:update-user", "修改当前用户信息")]
        [HttpPost("UpdateUser")]
        public IActionResult UpdateUser([FromForm] string? avatar, [FromForm] string? sex, [FromForm] string? email)
        {
            var userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRequest = new UpdateUserRequest()
            {
                AvatarData = avatar ?? string.Empty,
                Sex = sex ?? string.Empty,
                Email = email ?? string.Empty,
            };
            if (_identityCommandService.UpdateUser(userId, userRequest, out string msg))
            {
                return Ok(new ApiResponse { Code = 200, Data = string.Empty, Msg = msg });
            }
            return Ok(new ApiResponse { Code = 400, Data = string.Empty, Msg = "修改失败:" + msg });
        }
    }
}
