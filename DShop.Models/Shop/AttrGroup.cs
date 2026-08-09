using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 属性分组
    /// </summary>
    [Table("AttrGroups")]
    public class AttrGroup : ShopEntityBase
    {
        /// <summary>
        /// 所属分类Id
        /// </summary>
        public long CategoryId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Desc { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [MaxLength(500)]
        public string? Icon { get; set; }
    }
}
