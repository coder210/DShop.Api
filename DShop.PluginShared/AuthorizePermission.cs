namespace DShop.PluginShared
{
    /// <summary>
    /// 权限校验特性。
    /// permissionCode 不需要带端前缀（如 admin::），种子服务会按 Client 自动拼接前缀写入 PermissionCode。
    /// 约定：Client 默认 "admin"；未来 app 端控制器显式声明 Client = "app"。
    /// 端信息同时独立存入权限表 Client 列，便于按端查询/统计。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizePermissionAttribute : Attribute
    {
        public string Name { get; }
        public string PermissionCode { get; }
        /// <summary>
        /// 权限所属端，默认 "admin"。app 端控制器显式声明 Client = "app"。
        /// </summary>
        public string Client { get; }
        public AuthorizePermissionAttribute(string permissionCode, string name = "", string client = "admin")
        {
            PermissionCode = permissionCode;
            Name = name;
            Client = client;
        }
    }
}
