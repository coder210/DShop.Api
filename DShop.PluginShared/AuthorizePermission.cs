namespace DShop.PluginShared
{
    /// <summary>
    /// 权限校验特性。
    /// permissionCode 不需要带端前缀（如 admin::），Filter 会自动根据 Controller 所在的命名空间推导。
    /// 约定：命名空间包含 ".Admin." 时前缀为 "admin"，包含 ".App." 时前缀为 "app"。
    /// 若无法推导，则使用 permissionCode 原样校验。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizePermissionAttribute : Attribute
    {
        public string Name { get; }
        public string PermissionCode { get; }
        public AuthorizePermissionAttribute(string permissionCode, string name = "")
        {
            PermissionCode = permissionCode;
            Name = name;
        }
    }
}
