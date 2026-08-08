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
        // 1. 默认菜单（幂等：确保存在并修正 path/icon，使侧边栏跳转与前端路由一致）
        var defaultMenus = new List<(string Name, string Path, string Icon, string Controller, int SortOrder)>
        {
            ("首页", "/home", "HomeFilled", "", 1),
            ("用户管理", "/home/user-management", "User", "UserManagement", 2),
            ("角色管理", "/home/role-management", "UserFilled", "RoleManagement", 3),
            ("菜单管理", "/home/menu-management", "Menu", "MenuManagement", 4),
            ("模板管理", "/home/template-management", "Document", "TemplateManagement", 5),
            ("审计日志", "/home/audit-log-management", "Bell", "AuditLog", 6),
        };

        foreach (var dm in defaultMenus)
        {
            var existing = context.Menus.FirstOrDefault(m => m.Name == dm.Name);
            if (existing == null)
            {
                context.Menus.Add(new Menu
                {
                    Name = dm.Name,
                    Path = dm.Path,
                    Icon = dm.Icon,
                    Controller = dm.Controller,
                    ParentId = 0,
                    SortOrder = dm.SortOrder,
                    CreatedAt = DateTime.Now
                });
            }
            else
            {
                // 已存在则修正 path/icon/controller（解决旧数据 path 不匹配导致跳转 404）
                existing.Path = dm.Path;
                existing.Icon = dm.Icon;
                existing.Controller = dm.Controller;
                existing.SortOrder = dm.SortOrder;
            }
        }
        context.SaveChanges();

        // 2. 内置角色 admin（权限种子依赖该角色存在）
        long adminRoleId = 0;
        if (!context.Roles.Any(r => r.Code == "admin"))
        {
            var adminRole = new Role
            {
                Code = "admin",
                Name = "管理员",
                Description = "系统超级管理员，拥有全部权限",
                SortOrder = 0,
                IsSystem = true
            };
            context.Roles.Add(adminRole);
            context.SaveChanges();
            adminRoleId = adminRole.Id;
        }
        else
        {
            adminRoleId = context.Roles.First(r => r.Code == "admin").Id;
        }

        // 3. 管理员账号（用户名 admin / 密码 admin123）
        if (!context.Users.Any())
        {
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
                RoleId = adminRoleId,
            });
            context.SaveChanges();
        }

        // 4. 权限及角色-权限绑定
        var entryAsm = Assembly.GetEntryAssembly();
        var pluginAsms = new List<Assembly>();
        if (entryAsm != null) pluginAsms.Add(entryAsm);
        permissionSeed.SeedPermissions(pluginAsms);

        // 5. admin 角色绑定全部菜单（角色-菜单主授权链路）
        if (!context.RoleMenus.Any(rm => rm.RoleId == adminRoleId))
        {
            var allMenuIds = context.Menus.Select(m => m.Id).ToList();
            var roleMenus = allMenuIds
                .Select(menuId => new RoleMenu { RoleId = adminRoleId, MenuId = menuId })
                .ToList();
            context.RoleMenus.AddRange(roleMenus);
            context.SaveChanges();
        }
    }
}
