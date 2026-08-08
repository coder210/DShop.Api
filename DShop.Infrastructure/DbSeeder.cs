using DShop.Models;
using DShop.PluginShared;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DShop.Infrastructure;

/// <summary>
/// 数据库初始种子数据：首次启动时创建管理员账号、内置角色与默认菜单。
/// 权限及角色-权限绑定由 IPermissionSeedService 负责。
/// </summary>
public static class DbSeeder
{
    public static void Seed(DatabaseContext context, IPermissionSeedService permissionSeed)
    {
        // 1. 默认菜单（仅库为空时插入）
        if (!context.Menus.Any())
        {
            var menus = new List<Menu>
            {
                new Menu { Name = "首页", Path = "/home", Icon = "HomeFilled", ParentId = 0, Controller = "", SortOrder = 1, CreatedAt = DateTime.Now },
                new Menu { Name = "系统管理", Path = "/system", Icon = "Setting", ParentId = 0, Controller = "", SortOrder = 2, CreatedAt = DateTime.Now },
                new Menu { Name = "用户管理", Path = "/AccountManagement", Icon = "User", ParentId = 0, Controller = "UserManagement", SortOrder = 3, CreatedAt = DateTime.Now },
                new Menu { Name = "角色管理", Path = "/RoleManagement", Icon = "UserFilled", ParentId = 0, Controller = "RoleManagement", SortOrder = 4, CreatedAt = DateTime.Now },
                new Menu { Name = "菜单管理", Path = "/MenuManagement", Icon = "Menu", ParentId = 0, Controller = "MenuManagement", SortOrder = 5, CreatedAt = DateTime.Now },
                new Menu { Name = "模板管理", Path = "/TemplateManagement", Icon = "Document", ParentId = 0, Controller = "TemplateManagement", SortOrder = 6, CreatedAt = DateTime.Now },
                new Menu { Name = "审计日志", Path = "/AuditLog", Icon = "Bell", ParentId = 0, Controller = "AuditLogManagement", SortOrder = 7, CreatedAt = DateTime.Now },
            };
            context.Menus.AddRange(menus);
            context.SaveChanges();
        }

        // 2. 内置角色 admin（权限种子依赖该角色存在）
        if (!context.Roles.Any(r => r.Code == "admin"))
        {
            context.Roles.Add(new Role
            {
                Code = "admin",
                Name = "管理员",
                Description = "系统超级管理员，拥有全部权限",
                SortOrder = 0,
                IsSystem = true
            });
            context.SaveChanges();
        }

        // 3. 管理员账号（用户名 admin / 密码 admin123）
        if (!context.Users.Any())
        {
            var adminRole = context.Roles.First(r => r.Code == "admin");
            var user = new User
            {
                Username = "admin",
                PasswordHash = Md5Helper.ComputeMD5Hash("admin123"),
                Avatar = string.Empty,
                Email = string.Empty,
                CreatedAt = DateTime.Now,
                IdCard = string.Empty,
                Sex = "未知",
                MobilePhoneNumber = string.Empty,
                LastLoginAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = 0,
                ModifiedBy = 0,
                ModifiedAt = DateTime.Now,
            };
            context.Users.Add(user);
            context.SaveChanges();

            // 绑定 admin 角色
            context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id,
            });
            context.SaveChanges();
        }

        // 4. 权限及角色-权限绑定
        var entryAsm = Assembly.GetEntryAssembly();
        var pluginAsms = new List<Assembly>();
        if (entryAsm != null) pluginAsms.Add(entryAsm);
        permissionSeed.SeedPermissions(pluginAsms);
    }
}
