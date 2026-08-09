using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 商品SKU（最小库存单元）
    /// </summary>
    [Table("Skus")]
    public class Sku : ShopEntityBase
    {
        /// <summary>
        /// 所属SPU Id
        /// </summary>
        public long SpuId { get; set; }
        /// <summary>
        /// 规格图片
        /// </summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        /// <summary>
        /// 销售价（分）
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        public int SaleCount { get; set; }
        /// <summary>
        /// 条码
        /// </summary>
        [MaxLength(100)]
        public string? BarCode { get; set; }
        /// <summary>
        /// 二维码
        /// </summary>
        [MaxLength(100)]
        public string? QrCode { get; set; }
    }
}
