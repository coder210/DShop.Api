using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// SPU属性值
    /// </summary>
    [Table("SpuAttrValues")]
    public class SpuAttrValue : ShopEntityBase
    {
        /// <summary>
        /// 所属SPU Id
        /// </summary>
        public long SpuId { get; set; }
        /// <summary>
        /// 属性Id
        /// </summary>
        public long AttrId { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        [MaxLength(100)]
        public string? Name { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        [MaxLength(500)]
        public string? Value { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
    }
}
