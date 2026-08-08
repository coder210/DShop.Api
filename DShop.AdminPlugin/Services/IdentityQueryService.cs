using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.PluginShared;
using DShop.Models;
using Microsoft.Extensions.Configuration;

namespace DShop.AdminPlugin.Services
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

        /// <summary>
        /// 取用户可见菜单：角色菜单(主) ∪ 用户额外菜单(加成)。
        /// </summary>
        public List<Menu> GetUserMenus(long userId)
        {
            var roleIds = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            var roleMenuIds = _context.RoleMenus
                .Where(rm => roleIds.Contains(rm.RoleId))
                .Select(rm => rm.MenuId)
                .ToHashSet();

            var userMenuIds = _context.UserMenus
                .Where(um => um.UserId == userId)
                .Select(um => um.MenuId)
                .ToHashSet();

            var menuIds = roleMenuIds.Union(userMenuIds).ToList();
            return _context.Menus.Where(it => menuIds.Contains(it.Id)).ToList();
        }

        /// <summary>
        /// 取用户拥有的权限：角色权限(主) ∪ 用户额外权限(加成)。
        /// </summary>
        public List<Permission> GetUserPermissions(long userId)
        {
            var roleIds = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            var rolePermissionIds = _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .ToHashSet();

            var userPermissionIds = _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToHashSet();

            var permissionIds = rolePermissionIds.Union(userPermissionIds).ToList();
            return _context.Permissions.Where(it => permissionIds.Contains(it.Id)).ToList();
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
            };
        }

        // ==================== 角色查询 ====================

        public List<Role> GetRoles()
        {
            return _context.Roles.OrderBy(r => r.SortOrder).ToList();
        }

        public List<string> GetUserRoleCodes(long userId)
        {
            var roleIds = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();
            return _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Code)
                .ToList();
        }

        public List<long> GetUserRoleIds(long userId)
        {
            return _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();
        }

        public List<long> GetRoleMenus(long roleId)
        {
            return _context.RoleMenus
                .Where(rm => rm.RoleId == roleId)
                .Select(rm => rm.MenuId)
                .ToList();
        }

        public List<long> GetRolePermissions(long roleId)
        {
            return _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToList();
        }
    }
}
