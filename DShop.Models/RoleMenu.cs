using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 角色-菜单关联表（角色可见的菜单，决定前端导航显示）
    /// </summary>
    [Table("RoleMenus")]
    public class RoleMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long RoleId { get; set; }

        public long MenuId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }

        [ForeignKey(nameof(MenuId))]
        public Menu? Menu { get; set; }
    }
}
