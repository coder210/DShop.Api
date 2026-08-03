using System.ComponentModel.DataAnnotations;

namespace DShop.BasePlugin.Responses
{
    public class PermissionResponse
    {
        public long Id { get; set; }

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

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
