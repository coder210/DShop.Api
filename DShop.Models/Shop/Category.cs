using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 商品分类
    /// </summary>
    [Table("Categories")]
    public class Category : ShopEntityBase
    {
        /// <summary>
        /// 父分类Id（0为顶级）
        /// </summary>
        public long ParentId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 图标
        /// </summary>
        [MaxLength(500)]
        public string? Icon { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public CategoryStatus Status { get; set; }
    }
}
