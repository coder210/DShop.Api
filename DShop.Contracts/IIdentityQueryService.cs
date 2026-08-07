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
        List<Permission> GetUserPermissions(long userId);
        List<string> GetUserRoleCodes(long userId);
        List<Role> GetRoles();
        List<int> GetUserRoleIds(long userId);
        List<long> GetRoleMenus(int roleId);
        List<long> GetRolePermissions(int roleId);

        // ==================== 菜单查询 ====================
        List<MenuResponse> GetMenus(MenuQueryRequest query);
        MenuResponse GetMenuById(long id);
        List<MenuResponse> GetMenuTree();


        // ==================== 权限查询 ====================
        List<Permission> GetPermissions();
        bool GetPermissionsById(long id, out Permission permission);
        List<Permission> GetPermissionsByName(string name);
    }
}
