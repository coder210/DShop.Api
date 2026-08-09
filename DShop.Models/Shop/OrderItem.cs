using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 订单明细
    /// </summary>
    [Table("OrderItems")]
    public class OrderItem : ShopEntityBase
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [MaxLength(64)]
        public string OrderSn { get; set; } = string.Empty;
        /// <summary>
        /// 商品SPU Id
        /// </summary>
        public long SpuId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(200)]
        public string SpuName { get; set; } = string.Empty;
        /// <summary>
        /// 商品品牌
        /// </summary>
        [MaxLength(100)]
        public string? SpuBrand { get; set; }
        /// <summary>
        /// 分类Id
        /// </summary>
        public long CategoryId { get; set; }
        /// <summary>
        /// 商品SKU Id
        /// </summary>
        public long SkuId { get; set; }
        /// <summary>
        /// SKU名称
        /// </summary>
        [MaxLength(200)]
        public string? SkuName { get; set; }
        /// <summary>
        /// SKU图片
        /// </summary>
        [MaxLength(500)]
        public string? SkuPic { get; set; }
        /// <summary>
        /// SKU售价（分）
        /// </summary>
        public int SkuPrice { get; set; }
        /// <summary>
        /// SKU数量
        /// </summary>
        public int SkuQuantity { get; set; }
        /// <summary>
        /// SKU规格值（如 颜色:红;尺寸:XL）
        /// </summary>
        [MaxLength(500)]
        public string? SkuAttrsVals { get; set; }
        /// <summary>
        /// 促销优惠金额（分）
        /// </summary>
        public int PromotionAmount { get; set; }
        /// <summary>
        /// 优惠券优惠金额（分）
        /// </summary>
        public int CouponAmount { get; set; }
        /// <summary>
        /// 积分抵扣金额（分）
        /// </summary>
        public int IntegrationAmount { get; set; }
        /// <summary>
        /// 实付金额（分）
        /// </summary>
        public int RealAmount { get; set; }
        /// <summary>
        /// 赠送积分
        /// </summary>
        public int GiftIntegration { get; set; }
    }
}
