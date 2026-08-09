using DShop.Contracts.Dto;
using DShop.Models;

namespace DShop.Contracts
{
    public interface IIdentityQueryService
    {
        // ==================== 用户查询 ====================
        List<User> GetUserList();
        bool GetUser(long id, out User user, out string msg);
        bool GetValidatedToken(long id, out RefreshToken? tokenInfo);
        List<Menu> GetUserMenus(long userId);
        /// <summary>
        /// 取用户直接绑定的菜单（仅 UserMenus 表，不含角色菜单）。
        /// </summary>
        List<Menu> GetUserDirectMenus(long userId);
        /// <summary>
        /// 取用户所有能看到的菜单树，并标注每个菜单的来源（direct/role/both）。
        /// </summary>
        List<VisibleMenuResponse> GetUserVisibleMenus(long userId);
        /// <summary>
        /// 把菜单列表临时补全所有祖先菜单（仅用于构建展示树，不写回绑定表）。
        /// </summary>
        List<Menu> ExpandMenuAncestors(List<Menu> menus);
        List<Permission> GetUserPermissions(long userId);
        List<string> GetUserRoleCodes(long userId);
        List<Role> GetRoles();
        List<long> GetUserRoleIds(long userId);
        List<long> GetRoleMenus(long roleId);
        List<long> GetRolePermissions(long roleId);

        // ==================== 菜单查询 ====================
        List<MenuResponse> GetMenus(MenuQueryRequest query);
        MenuResponse GetMenuById(long id);
        List<MenuResponse> GetMenuTree(long? currentUserId = null);


        // ==================== 权限查询 ====================
        List<Permission> GetPermissions();
        bool GetPermissionsById(long id, out Permission permission);
        List<Permission> GetPermissionsByName(string name);
    }
}
