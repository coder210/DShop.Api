using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 角色表
    /// </summary>
    [Table("Roles")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 角色编码，全局唯一，如 sale / sampler / admin
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 角色名称，如 管理员 / 总工 / 普通用户
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(256)]
        public string? Description { get; set; }

        /// <summary>
        /// 排序号，越小越靠前
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// 是否内置角色（内置角色不可删除）
        /// </summary>
        public bool IsSystem { get; set; } = false;
    }
}
