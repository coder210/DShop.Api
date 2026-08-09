using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 客户收藏
    /// </summary>
    [Table("CollectSpus")]
    public class CollectSpu : ShopEntityBase
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public long CustomerId { get; set; }
        /// <summary>
        /// 商品SPU Id
        /// </summary>
        public long SpuId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(200)]
        public string? SpuName { get; set; }
        /// <summary>
        /// 商品价格（分）
        /// </summary>
        public int SpuPrice { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [MaxLength(500)]
        public string? SpuImageUrl { get; set; }
    }
}
