using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 商品品牌
    /// </summary>
    [Table("Brands")]
    public class Brand : ShopEntityBase
    {
        /// <summary>
        /// 品牌名称
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Logo
        /// </summary>
        [MaxLength(500)]
        public string? Logo { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Desc { get; set; }
        /// <summary>
        /// 首字母（用于拼音索引）
        /// </summary>
        [MaxLength(10)]
        public string? FirstLetter { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public BrandStatus Status { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
    }
}
