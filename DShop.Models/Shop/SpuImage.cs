using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// SPU图片
    /// </summary>
    [Table("SpuImages")]
    public class SpuImage : ShopEntityBase
    {
        /// <summary>
        /// 所属SPU Id
        /// </summary>
        public long SpuId { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
    }
}
