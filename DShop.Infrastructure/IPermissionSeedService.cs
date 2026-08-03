using System.Reflection;

namespace DShop.Infrastructure;

/// <summary>启动时扫描程序集中的 [AuthorizePermission] 并同步到权限表</summary>
public interface IPermissionSeedService
{
    void SeedPermissions(IEnumerable<Assembly> additionalAssemblies);
}
