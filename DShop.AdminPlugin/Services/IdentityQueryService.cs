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
        /// 取用户直接绑定的菜单（仅 UserMenus 表，不含角色菜单）。
        /// 用于用户管理"绑定菜单"，语义是只管理用户自己的额外菜单。
        /// </summary>
        public List<Menu> GetUserDirectMenus(long userId)
        {
            var menuIds = _context.UserMenus
                .Where(um => um.UserId == userId)
                .Select(um => um.MenuId)
                .ToList();
            return _context.Menus.Where(it => menuIds.Contains(it.Id)).ToList();
        }

        /// <summary>
        /// 取用户所有能看到的菜单树，并标注每个菜单的来源（direct/role/both）。
        /// 先取可见菜单并补全祖先，再按 UserMenus / RoleMenus 归属判断来源。
        /// </summary>
        public List<VisibleMenuResponse> GetUserVisibleMenus(long userId)
        {
            var roleIds = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            var directIds = _context.UserMenus
                .Where(um => um.UserId == userId)
                .Select(um => um.MenuId)
                .ToHashSet();

            var roleMenuIds = _context.RoleMenus
                .Where(rm => roleIds.Contains(rm.RoleId))
                .Select(rm => rm.MenuId)
                .ToHashSet();

            // 可见菜单 = 直接绑定 ∪ 角色菜单，并临时补全祖先（祖先本身不算有绑定来源，标记为 role 占位由前端可读）
            var visible = _context.Menus
                .Where(it => directIds.Contains(it.Id) || roleMenuIds.Contains(it.Id))
                .ToList();
            visible = ExpandMenuAncestors(visible);

            var rootMenus = visible.Where(m => m.ParentId == 0).OrderBy(m => m.SortOrder).ToList();
            return BuildVisibleTree(rootMenus, visible, directIds, roleMenuIds);
        }

        private List<VisibleMenuResponse> BuildVisibleTree(
            List<Menu> parentNodes,
            List<Menu> allMenus,
            HashSet<long> directIds,
            HashSet<long> roleMenuIds)
        {
            var result = new List<VisibleMenuResponse>();

            foreach (var parent in parentNodes)
            {
                var node = new VisibleMenuResponse
                {
                    Id = parent.Id,
                    Name = parent.Name,
                    Path = parent.Path,
                    Icon = parent.Icon,
                    SortOrder = parent.SortOrder,
                    Controller = parent.Controller,
                    Source = ResolveSource(parent.Id, directIds, roleMenuIds)
                };
                var children = allMenus.Where(m => m.ParentId == parent.Id).OrderBy(m => m.SortOrder).ToList();
                node.Children = BuildVisibleTree(children, allMenus, directIds, roleMenuIds);
                result.Add(node);
            }

            return result;
        }

        private static string ResolveSource(long menuId, HashSet<long> directIds, HashSet<long> roleMenuIds)
        {
            var isDirect = directIds.Contains(menuId);
            var isRole = roleMenuIds.Contains(menuId);
            return isDirect && isRole ? "both" : (isDirect ? "direct" : (isRole ? "role" : "ancestor"));
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

        /// <summary>
        /// 把传入菜单临时补全其所有祖先（直到 ParentId == 0 的顶级菜单）。
        /// 仅用于构建展示树，不写回绑定表，保证勾选子菜单时导航/菜单树能向上展开。
        /// </summary>
        public List<Menu> ExpandMenuAncestors(List<Menu> menus)
        {
            if (menus == null || menus.Count == 0)
            {
                return menus ?? new List<Menu>();
            }

            var allMenus = _context.Menus.ToList();
            var menuMap = allMenus.ToDictionary(m => m.Id, m => m);
            var allowed = new HashSet<long>(menus.Select(m => m.Id));

            foreach (var menu in menus)
            {
                var pid = menu.ParentId;
                while (pid != 0 && menuMap.ContainsKey(pid))
                {
                    if (!allowed.Add(pid))
                    {
                        break;
                    }
                    pid = menuMap[pid].ParentId;
                }
            }

            return allMenus.Where(m => allowed.Contains(m.Id)).ToList();
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

        public List<MenuResponse> GetMenuTree(long? currentUserId = null)
        {
            var allMenus = _context.Menus.ToList();

            // 按当前登录用户可见菜单过滤（角色/用户分配菜单时，避免越权分配自己看不到的菜单）
            if (currentUserId.HasValue && currentUserId.Value > 0)
            {
                var visible = GetUserMenus(currentUserId.Value);
                var allowed = new HashSet<long>(visible.Select(m => m.Id));
                // 把可见菜单的祖先一并纳入，维持树结构连续
                foreach (var menu in visible)
                {
                    var pid = menu.ParentId;
                    while (pid != 0 && !allowed.Contains(pid))
                    {
                        allowed.Add(pid);
                        var parent = allMenus.FirstOrDefault(x => x.Id == pid);
                        if (parent == null) break;
                        pid = parent.ParentId;
                    }
                }
                allMenus = allMenus.Where(m => allowed.Contains(m.Id)).ToList();
            }

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
