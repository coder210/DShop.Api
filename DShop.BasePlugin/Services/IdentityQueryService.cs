using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.PluginShared;
using DShop.Models;
using Microsoft.Extensions.Configuration;

namespace DShop.BasePlugin.Services
{
    public class IdentityQueryService : IIdentityQueryService
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITokenValidator _tokenValidator;

        public IdentityQueryService(DatabaseContext context, IConfiguration configuration, ITokenValidator tokenValidator)
        {
            _context = context;
            _configuration = configuration;
            _tokenValidator = tokenValidator;
        }

        // ==================== 用户查询 ====================

        public List<User> GetUserList()
        {
            string basePath = _configuration[Constants.FileStorageBasePath].ToString();
            var userList = _context.Users.Where(it => !it.IsDeleted).ToList();
            foreach (var item in userList)
            {
                string fullDir = basePath + item.Avatar;
                item.Avatar = ImageToBase64.GetBase64FromImage(fullDir);
            }
            return userList;
        }

        public bool GetUser(long id, out User user, out string msg)
        {
            var userInfo = _context.Users.FirstOrDefault(it => it.Id == id);
            if (userInfo != null)
            {
                string basePath = _configuration[Constants.FileStorageBasePath].ToString();
                string fullDir = basePath + userInfo.Avatar;
                userInfo.Avatar = ImageToBase64.GetBase64FromImage(fullDir);
                user = userInfo;
                msg = "获取成功";
                return true;
            }
            else
            {
                user = userInfo;
                msg = "用户不存在";
                return false;
            }
        }

        public bool GetValidatedToken(long id, out RefreshToken? tokenInfo)
        {
            return _tokenValidator.ValidateToken(id, out tokenInfo);
        }

        public List<Menu> GetUserMenus(long userId)
        {
            // 角色菜单 ∪ 用户直绑菜单（并集去重）
            var roleMenuIds = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .SelectMany(roleId => _context.RoleMenus
                    .Where(rm => rm.RoleId == roleId)
                    .Select(rm => rm.MenuId))
                .ToList();

            var userMenuIds = _context.UserMenus
                .Where(it => it.UserId == userId)
                .Select(it => it.MenuId)
                .ToList();

            var menuIds = roleMenuIds.Union(userMenuIds).Distinct().ToList();
            return _context.Menus.Where(it => menuIds.Contains(it.Id)).ToList();
        }

        public List<Permission> GetUserPermissions(long userId)
        {
            // 角色权限 ∪ 用户直绑权限（并集去重）
            var rolePermissionIds = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .SelectMany(roleId => _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Select(rp => rp.PermissionId))
                .ToList();

            var userPermissionIds = _context.UserPermissions
                .Where(it => it.UserId == userId)
                .Select(it => it.PermissionId)
                .ToList();

            var permissionIds = rolePermissionIds.Union(userPermissionIds).Distinct().ToList();
            // 仅返回接口仍然存在的权限（IsActive=true）；代码中已删除的接口（IsActive=false）不进入用户权限，
            // 从而不会写入 JWT，鉴权过滤器会拒绝访问这些已失效接口。
            return _context.Permissions
                .Where(it => permissionIds.Contains(it.Id) && it.IsActive)
                .ToList();
        }

        /// <summary>
        /// 获取用户拥有的角色编码列表（取所有角色的并集）
        /// </summary>
        public List<string> GetUserRoleCodes(long userId)
        {
            return _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .Join(_context.Roles, ur => ur, r => r.Id, (_, r) => r.Code)
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToList();
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.OrderBy(r => r.SortOrder).ToList();
        }

        public List<int> GetUserRoleIds(long userId)
        {
            return _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .Distinct()
                .ToList();
        }

        public List<long> GetRoleMenus(int roleId)
        {
            return _context.RoleMenus
                .Where(rm => rm.RoleId == roleId)
                .Select(rm => rm.MenuId)
                .Distinct()
                .ToList();
        }

        public List<long> GetRolePermissions(int roleId)
        {
            return _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .Distinct()
                .ToList();
        }

        // ==================== 菜单查询 ====================

        public List<MenuResponse> GetMenus(MenuQueryRequest query)
        {
            var menus = _context.Menus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                menus = menus.Where(m => m.Name.Contains(query.Name));

            if (query.ParentId.HasValue)
                menus = menus.Where(m => m.ParentId == query.ParentId.Value);

            menus = query.SortBy?.ToLower() switch
            {
                "name" => query.IsDescending ? menus.OrderByDescending(m => m.Name) : menus.OrderBy(m => m.Name),
                "createdat" => query.IsDescending ? menus.OrderByDescending(m => m.CreatedAt) : menus.OrderBy(m => m.CreatedAt),
                _ => query.IsDescending ? menus.OrderByDescending(m => m.SortOrder) : menus.OrderBy(m => m.SortOrder)
            };

            menus = menus.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);

            return menus.Select(m => MapMenuToResponse(m)).ToList();
        }

        public MenuResponse GetMenuById(long id)
        {
            var menu = _context.Menus.Find(id);
            return menu == null ? null : MapMenuToResponse(menu);
        }

        public List<MenuResponse> GetMenuTree()
        {
            var allMenus = _context.Menus.ToList();
            var rootMenus = allMenus.Where(m => m.ParentId == 0).OrderBy(m => m.SortOrder);
            return BuildTree(rootMenus, allMenus);
        }

        // ==================== 权限查询 ====================

        public List<Permission> GetPermissions()
        {
            return _context.Permissions.ToList();
        }

        public bool GetPermissionsById(long id, out Permission permission)
        {
            permission = _context.Permissions.Where(it => it.Id == id).FirstOrDefault();
            return permission != null;
        }

        public List<Permission> GetPermissionsByName(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return new List<Permission>();
            return _context.Permissions
                .Where(p => p.Description.Contains(description))
                .ToList();
        }

        // ==================== 私有辅助方法 ====================

        private List<MenuResponse> BuildTree(IEnumerable<Menu> parentNodes, List<Menu> allMenus)
        {
            var result = new List<MenuResponse>();

            foreach (var parent in parentNodes)
            {
                var node = MapMenuToResponse(parent);
                var children = allMenus.Where(m => m.ParentId == parent.Id).OrderBy(m => m.SortOrder);
                node.Children = BuildTree(children, allMenus);
                result.Add(node);
            }

            return result;
        }

        private static MenuResponse MapMenuToResponse(Menu menu)
        {
            return new MenuResponse
            {
                Id = menu.Id,
                Name = menu.Name,
                Path = menu.Path,
                Icon = menu.Icon,
                SortOrder = menu.SortOrder,
                Controller = menu.Controller,
            };
        }
    }
}
