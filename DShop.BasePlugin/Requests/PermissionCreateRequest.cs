namespace DShop.BasePlugin.Requests
{
    public class PermissionCreateRequest
    {
        /// <summary>
        /// 权限标识：如 'quote:fc:view'
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
    }
}
