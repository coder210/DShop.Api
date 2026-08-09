namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 用户可见权限（含来源标注）。
    /// source 取值：direct=直接绑定(UserPermissions) / role=来自角色(RolePermissions) / both=两者都有
    /// </summary>
    public class VisiblePermissionResponse
    {
        public long Id { get; set; }
        public string PermissionCode { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 归属模块（kebab-case），如 user、role-management。
        /// </summary>
        public string Module { get; set; }
        public string Endpoint { get; set; }
        public string ApiPath { get; set; }
        /// <summary>
        /// 权限来源：direct / role / both
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 该权限来自的角色名称列表（来自角色时非空）。
        /// </summary>
        public List<string> RoleNames { get; set; }
    }
}
