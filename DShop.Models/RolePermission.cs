using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 角色-权限关联表（角色拥有的操作权限）
    /// </summary>
    [Table("RolePermissions")]
    public class RolePermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long RoleId { get; set; }

        public long PermissionId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public Permission? Permission { get; set; }
    }
}
