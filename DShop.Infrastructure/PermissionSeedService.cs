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
        var assemblies = new List<Assembly> { Assembly.GetEntryAssembly()! };
        foreach (var referenced in Assembly.GetEntryAssembly()!.GetReferencedAssemblies())
        {
            try
            {
                assemblies.Add(Assembly.Load(referenced));
            }
            catch { }
        }

        assemblies.AddRange(additionalAssemblies);

        var controllers = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);

        var permissionMap = new Dictionary<string, string>();

        foreach (var controller in controllers)
        {
            string? modulePrefix = null;
            var ns = controller.Namespace;

            if (ns != null)
            {
                if (ns.Contains(".Admin"))
                    modulePrefix = "admin";
                else if (ns.Contains(".App"))
                    modulePrefix = "app";
            }

            var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var action in actions)
            {
                var actionAttr = action.GetCustomAttribute<AuthorizePermissionAttribute>();
                if (actionAttr != null)
                {
                    var code = modulePrefix != null
                        ? $"{modulePrefix}::{actionAttr.PermissionCode}"
                        : actionAttr.PermissionCode;
                    var name = actionAttr.Name ?? code;
                    permissionMap[code] = name;
                }
            }
        }

        var allPermissions = _context.Permissions.ToList();
        var existingPermissionDict = allPermissions
            .ToDictionary(p => p.PermissionCode);

        // 标记代码中已删除的权限
        foreach (var perm in allPermissions)
        {
            if (!permissionMap.ContainsKey(perm.PermissionCode))
            {
                if (perm.Remark != "实体没有该标识")
                {
                    perm.Remark = "实体没有该标识";
                    perm.UpdatedAt = DateTime.Now;
                }
            }
        }

        foreach (var kv in permissionMap)
        {
            var code = kv.Key;
            var description = kv.Value;

            if (existingPermissionDict.TryGetValue(code, out var existing))
            {
                var hasChanged = false;

                if (existing.Description != description)
                {
                    existing.Description = description;
                    hasChanged = true;
                }

                if (existing.Remark == "实体没有该标识")
                {
                    existing.Remark = "实体没有该标识(fixed)";
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
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Remark = "更新成功",
                    SortOrder = 0,
                });
            }
        }

        _context.SaveChanges();
    }
}
