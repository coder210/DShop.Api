using System.Reflection;

namespace DShop.Infrastructure;

/// <summary>菜单-控制器-权限一致性校验报告</summary>
public class ControllerPermissionReport
{
    /// <summary>菜单声明了控制器，但库中缺失的权限（菜单应有、库里没有）</summary>
    public List<MissingPermissionItem> MissingInDb { get; set; } = new();
    /// <summary>库里存在权限，但没有任何菜单声明对应控制器的孤立权限</summary>
    public List<OrphanPermissionItem> OrphanPermissions { get; set; } = new();
    /// <summary>菜单声明了控制器，但代码里已不存在该控制器</summary>
    public List<UnknownControllerItem> UnknownControllers { get; set; } = new();
}

public class MissingPermissionItem
{
    public string MenuName { get; set; }
    public string Controller { get; set; }
    public List<string> MissingActions { get; set; } = new();
}

public class OrphanPermissionItem
{
    public string Module { get; set; }
    public List<string> PermissionCodes { get; set; } = new();
}

public class UnknownControllerItem
{
    public string MenuName { get; set; }
    public string Controller { get; set; }
}

/// <summary>启动时扫描程序集中的 [AuthorizePermission] 并同步到权限表</summary>
public interface IPermissionSeedService
{
    void SeedPermissions(IEnumerable<Assembly> additionalAssemblies);
    /// <summary>校验菜单声明的控制器与其权限是否一致（菜单有控制器、权限表 Module 是否齐全等）</summary>
    ControllerPermissionReport GetControllerPermissionReport();
}
