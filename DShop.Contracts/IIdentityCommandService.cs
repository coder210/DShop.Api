using DShop.Contracts.Dto;
using DShop.Models;
using System.Reflection;

namespace DShop.Contracts
{
    public interface IIdentityCommandService
    {
        // ==================== 用户管理 ====================
        bool DeleteUser(long id, out string msg);
        LoginResponse Login(string username, string password, string captcha, string deviceInfo = "");
        bool Register(string username, string password, out string msg);
        bool ForgotPassword(string username, string oldPassword, string newPassword, string captcha, out string msg);
        bool UpdatePassword(long id, string newPassword, string captcha, out string msg);
        bool Logout(long id, out string msg);
        bool UpdateUser(long id, UpdateUserRequest userRequest, out string msg);

        // ==================== 用户-菜单绑定 ====================
        bool BindMenuList(long userId, List<long> menuIdList);

        // ==================== 用户-权限绑定 ====================
        (bool Success, string Message) BindPermissionList(long userId, List<long> permissionIdList);

        // ==================== 用户-角色绑定 ====================
        (bool Success, string Message) BindRoleList(long userId, List<int> roleIdList);

        // ==================== 角色管理 ====================
        long CreateRole(Role role, out string msg);
        bool UpdateRole(Role role, out string msg);
        bool DeleteRole(long id, out string msg);
        bool BindRoleMenus(long roleId, List<long> menuIds);
        bool BindRolePermissions(long roleId, List<long> permissionIds);

        // ==================== 菜单管理 ====================
        bool AddMenu(MenuCreateRequest request);
        bool UpdateMenu(MenuUpdateRequest request, out string msg);
        bool DeleteMenu(long id, out string msg);
        bool DeleteMenus(IEnumerable<long> ids, out string msg);

        // ==================== 菜单-权限绑定 ====================


        // ==================== 权限管理 ====================
        bool AddPermission(Permission permission, out string msg);
        bool AddPermissions(IEnumerable<Permission> permissions, out string msg);
        bool UpdatePermission(Permission permission, out string msg);
        bool DeletePermission(long id, out string msg);
        bool DeletePermissions(IEnumerable<long> ids, out string msg);
        void SeedPermissions();
        void SeedPermissions(IEnumerable<Assembly> additionalAssemblies);
    }
}
