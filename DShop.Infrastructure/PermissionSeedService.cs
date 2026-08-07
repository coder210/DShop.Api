using DShop.Models;
using DShop.PluginShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DShop.Infrastructure;

public class PermissionSeedService : IPermissionSeedService
{
    private readonly DatabaseContext _context;

    public PermissionSeedService(DatabaseContext context)
    {
        _context = context;
    }

    public void SeedPermissions(IEnumerable<Assembly> additionalAssemblies)
    {
        var assemblies = CollectAssemblies(additionalAssemblies);

        var controllers = GetControllerTypes(assemblies);

        // 权限码规范：kebab-case 的 {module}:{action}，按 Client 加端前缀。
        // 例如 user:change-password -> admin::user:change-password
        var permissionMap = new Dictionary<string, (string name, string module, string endpoint, string apiPath, string client)>();

        foreach (var controller in controllers)
        {
            // 控制器路由模板：如 "api/admin/[controller]" -> "api/admin/Home"
            var controllerRoute = controller.GetCustomAttribute<RouteAttribute>()?.Template ?? "";
            var controllerName = controller.Name.Replace("Controller", "", StringComparison.Ordinal);
            var controllerRouteBase = controllerRoute.Replace("[controller]", controllerName, StringComparison.OrdinalIgnoreCase).TrimStart('/');

            var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var action in actions)
            {
                var actionAttr = action.GetCustomAttribute<AuthorizePermissionAttribute>();
                if (actionAttr != null)
                {
                    // 端前缀：直接读取特性声明的 Client（默认 admin），不再依赖命名空间推导。
                    var client = actionAttr.Client ?? "admin";
                    var modulePrefix = client;

                    var code = modulePrefix != null
                        ? $"{modulePrefix}::{actionAttr.PermissionCode}"
                        : actionAttr.PermissionCode;
                    var name = actionAttr.Name ?? code;
                    // 归属模块 = 控制器名称去 "Controller" 后缀，如 UserManagementController -> UserManagement。
                    // 与菜单表 Menu.Controller 使用同一套取值，建立菜单与权限的关联契约。
                    // 注意：权限码 PermissionCode 本身保持 kebab-case 不变，
                    // 仅 Module 列采用控制器名，用于菜单与权限的关联与反查。
                    var module = controllerName;

                    // Endpoint：控制器类名.方法名，便于后端直接定位代码（与路由无关）
                    var endpoint = $"{controller.Name}.{action.Name}";

                    // ApiPath：HTTP 方法 + 路由模板，与前端 Network 看到的 URL 一致
                    var httpMethod = GetHttpMethod(action);
                    var actionRoute = GetActionRouteTemplate(action);
                    var fullRoute = string.IsNullOrEmpty(actionRoute)
                        ? controllerRouteBase
                        : $"{controllerRouteBase}/{actionRoute.TrimStart('/')}";
                    var apiPath = $"{httpMethod} /{fullRoute}";

                    permissionMap[code] = (name, module, endpoint, apiPath, client);
                }
            }
        }

        var allPermissions = _context.Permissions.ToList();
        var existingPermissionDict = allPermissions
            .ToDictionary(p => p.PermissionCode);

        // 标记代码中已删除的权限：代码里不再存在的接口，IsActive 置为 false（不物理删除，保留角色绑定历史）
        foreach (var perm in allPermissions)
        {
            if (!permissionMap.ContainsKey(perm.PermissionCode))
            {
                if (perm.IsActive)
                {
                    perm.IsActive = false;
                    perm.UpdatedAt = DateTime.Now;
                }
            }
        }

        foreach (var kv in permissionMap)
        {
            var code = kv.Key;
            var (description, module, endpoint, apiPath, client) = kv.Value;

            if (existingPermissionDict.TryGetValue(code, out var existing))
            {
                var hasChanged = false;

                if (existing.Description != description)
                {
                    existing.Description = description;
                    hasChanged = true;
                }

                if (existing.Module != module)
                {
                    existing.Module = module;
                    hasChanged = true;
                }

                if (existing.Client != client)
                {
                    existing.Client = client;
                    hasChanged = true;
                }

                if (existing.Endpoint != endpoint)
                {
                    existing.Endpoint = endpoint;
                    hasChanged = true;
                }

                if (existing.ApiPath != apiPath)
                {
                    existing.ApiPath = apiPath;
                    hasChanged = true;
                }

                // 接口在代码里重新出现：恢复为有效，并清理旧的失效标记
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    hasChanged = true;
                }

                // 清除历史失效备注（若曾因删除被标记）
                if (!string.IsNullOrEmpty(existing.Remark) && existing.Remark.Contains("实体没有该标识"))
                {
                    existing.Remark = "更新成功";
                    hasChanged = true;
                }

                if (hasChanged)
                {
                    existing.UpdatedAt = DateTime.Now;
                }
            }
            else
            {
                _context.Permissions.Add(new Permission
                {
                    PermissionCode = code,
                    Description = description,
                    Module = module,
                    Client = client,
                    Endpoint = endpoint,
                    ApiPath = apiPath,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Remark = "更新成功",
                    SortOrder = 0,
                });
            }
        }

        _context.SaveChanges();

        // 角色默认权限分配：根据角色编码，把匹配的权限前缀批量授予角色
        SeedRolePermissions();
    }

    /// <summary>
    /// 取 Action 的 HTTP 方法（GET/POST/PUT/DELETE），默认 GET。
    /// </summary>
    private static string GetHttpMethod(MethodInfo action)
    {
        if (action.GetCustomAttribute<HttpPostAttribute>() != null) return "POST";
        if (action.GetCustomAttribute<HttpPutAttribute>() != null) return "PUT";
        if (action.GetCustomAttribute<HttpDeleteAttribute>() != null) return "DELETE";
        if (action.GetCustomAttribute<HttpPatchAttribute>() != null) return "PATCH";
        return "GET";
    }

    /// <summary>
    /// 取 Action 的路由模板片段（如 "GetList"、"Create"、"{id}"），无则空字符串。
    /// 多个路由特性时取第一个。
    /// </summary>
    private static string GetActionRouteTemplate(MethodInfo action)
    {
        var routeAttr = action.GetCustomAttribute<RouteAttribute>();
        if (routeAttr != null && !string.IsNullOrEmpty(routeAttr.Template)) return routeAttr.Template;

        // HTTP 方法特性（HttpGet/Post/Put/Delete 等）均继承自 HttpMethodAttribute，各自可能带路由模板
        foreach (var attr in action.GetCustomAttributes())
        {
            if (attr is HttpGetAttribute g && !string.IsNullOrEmpty(g.Template)) return g.Template;
            if (attr is HttpPostAttribute p && !string.IsNullOrEmpty(p.Template)) return p.Template;
            if (attr is HttpPutAttribute u && !string.IsNullOrEmpty(u.Template)) return u.Template;
            if (attr is HttpDeleteAttribute d && !string.IsNullOrEmpty(d.Template)) return d.Template;
            if (attr is HttpPatchAttribute pa && !string.IsNullOrEmpty(pa.Template)) return pa.Template;
        }
        return "";
    }

    /// <summary>
    /// 将权限按模块分配给内置角色。Module 值与菜单表 Menu.Controller 使用同一套取值（控制器名去后缀）。
    /// 角色 Code 与 Module 的映射随系统功能演进维护。
    /// </summary>
    private void SeedRolePermissions()
    {
        // 角色编码 -> 该角色拥有的权限 Module 列表。
        // Module 即控制器名去 "Controller" 后缀，如 UserManagement、MenuManagement。
        var rolePermissionModules = new Dictionary<string, string[]>
        {
            // 管理员：系统全部权限（首页 + 账号/角色/菜单管理 + 模板 + 审计）
            ["admin"] = new[] { "Home", "UserManagement", "RoleManagement", "MenuManagement",
                "TemplateManagement", "AuditLogManagement" },
            // 总工：查看类权限（首页 + 模板 + 审计）
            ["chief-engineer"] = new[] { "Home", "TemplateManagement", "AuditLogManagement" },
        };

        foreach (var (roleCode, modules) in rolePermissionModules)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Code == roleCode);
            if (role == null) continue;

            var moduleSet = modules.ToHashSet();
            var matchedPermissionIds = _context.Permissions
                .AsEnumerable()
                .Where(p => p.Module != null && moduleSet.Contains(p.Module))
                .Select(p => p.Id)
                .ToList();

            var existingRolePermissionIds = _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.PermissionId)
                .ToHashSet();

            foreach (var permissionId in matchedPermissionIds)
            {
                if (existingRolePermissionIds.Contains(permissionId)) continue;
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                });
            }
        }

        _context.SaveChanges();
    }

    /// <summary>
    /// 收集用于控制器扫描的程序集：入口程序集 + 其引用 + 插件程序集。
    /// </summary>
    private static List<Assembly> CollectAssemblies(IEnumerable<Assembly> additionalAssemblies)
    {
        var entry = Assembly.GetEntryAssembly()!;
        var assemblies = new List<Assembly> { entry };
        foreach (var referenced in entry.GetReferencedAssemblies())
        {
            try { assemblies.Add(Assembly.Load(referenced)); }
            catch { }
        }
        assemblies.AddRange(additionalAssemblies);
        return assemblies;
    }

    /// <summary>
    /// 从程序集中取出所有非抽象 Controller 类型。
    /// </summary>
    private static IEnumerable<Type> GetControllerTypes(IEnumerable<Assembly> assemblies)
    {
        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);
    }

    /// <summary>
    /// 校验菜单声明的控制器与其权限是否一致（纯数据库判断，不扫描程序集，避免波及插件热更新）：
    /// ① 菜单声明了控制器，但库里该 Module 没有任何权限记录（菜单应有、库里没有）；
    /// ② 库里存在权限，但没有任何菜单声明对应控制器（孤立权限）；
    /// ③ 菜单声明了控制器，但权限表里完全找不到该 Module 的记录（脏数据）。
    /// 只读，不修改任何数据。
    /// 注意：不依赖代码反射，因此无法精确判断"少了某几个 Action 权限"，
    /// 仅以"权限表是否存在该 Module 记录"作为存在性依据。
    /// </summary>
    public ControllerPermissionReport GetControllerPermissionReport()
    {
        var report = new ControllerPermissionReport();
        var menus = _context.Menus.ToList();
        var permissions = _context.Permissions.ToList();

        // 库里权限按 Module 分组（仅取有权限记录的 Module）
        var permissionsByModule = permissions
            .Where(p => !string.IsNullOrEmpty(p.Module))
            .GroupBy(p => p.Module, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Select(p => p.PermissionCode).ToList(), StringComparer.OrdinalIgnoreCase);

        var modulesWithMenu = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var menu in menus)
        {
            if (string.IsNullOrWhiteSpace(menu.Controller)) continue;
            var controller = menu.Controller.Trim();
            modulesWithMenu.Add(controller);

            // 权限表里是否存在该 Module 的记录
            var hasModule = permissionsByModule.ContainsKey(controller);

            if (!hasModule)
            {
                // ③ 菜单声明了控制器，但库里完全没有该 Module 的权限记录（脏数据）
                // 同时归入 ①：菜单应有、库里没有
                report.UnknownControllers.Add(new UnknownControllerItem
                {
                    MenuName = menu.Name,
                    Controller = controller,
                });
                report.MissingInDb.Add(new MissingPermissionItem
                {
                    MenuName = menu.Name,
                    Controller = controller,
                    MissingActions = new List<string> { $"（权限表无 Module={controller} 的任何记录）" },
                });
            }
        }

        // ② 孤立权限：Module 没有任何菜单声明
        foreach (var (module, codes) in permissionsByModule)
        {
            if (!modulesWithMenu.Contains(module))
            {
                report.OrphanPermissions.Add(new OrphanPermissionItem
                {
                    Module = module,
                    PermissionCodes = codes,
                });
            }
        }

        return report;
    }
}
