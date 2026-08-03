using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 菜单权限表
    /// </summary>
    [Table("MenuPermissions")]
    public class MenuPermission
    {
        [Key]
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 菜单id
        /// </summary>
        public long MenuId { get; set; }
        /// <summary>
        /// 权限id
        /// </summary>
        public long PermissionId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
