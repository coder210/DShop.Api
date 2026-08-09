using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 商品属性
    /// </summary>
    [Table("Attrs")]
    public class Attr : ShopEntityBase
    {
        /// <summary>
        /// 所属分类Id
        /// </summary>
        public long CategoryId { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 是否需要检索
        /// </summary>
        public AttrSearchType SearchType { get; set; }
        /// <summary>
        /// 值类型（单个值/多个值）
        /// </summary>
        public AttrValueType ValueType { get; set; }
        /// <summary>
        /// 属性类型（销售/基本/两者）
        /// </summary>
        public AttrType AttrType { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [MaxLength(500)]
        public string? Icon { get; set; }
        /// <summary>
        /// 可选值（逗号分隔）
        /// </summary>
        public string? ValueSelect { get; set; }
        /// <summary>
        /// 是否展示在介绍上（0-否 1-是）
        /// </summary>
        public bool ShowDesc { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public AttrStatus Status { get; set; }
    }
}
