using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 订单
    /// </summary>
    [Table("Orders")]
    public class Order : ShopEntityBase
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public long CustomerId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [MaxLength(64)]
        public string OrderSn { get; set; } = string.Empty;
        /// <summary>
        /// 优惠券Id
        /// </summary>
        public long CouponId { get; set; }
        /// <summary>
        /// 客户手机号
        /// </summary>
        [MaxLength(20)]
        public string? CustomerMobile { get; set; }
        /// <summary>
        /// 商品总额（分）
        /// </summary>
        public int TotalAmount { get; set; }
        /// <summary>
        /// 应付金额（分）
        /// </summary>
        public int PayAmount { get; set; }
        /// <summary>
        /// 运费（分）
        /// </summary>
        public int FreightAmount { get; set; }
        /// <summary>
        /// 促销优惠金额（分）
        /// </summary>
        public int PromotionAmount { get; set; }
        /// <summary>
        /// 积分抵扣金额（分）
        /// </summary>
        public int IntegrationAmount { get; set; }
        /// <summary>
        /// 优惠券抵扣金额（分）
        /// </summary>
        public int CouponAmount { get; set; }
        /// <summary>
        /// 折扣金额（分）
        /// </summary>
        public int DiscountAmount { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public OrderPayType PayType { get; set; }
        /// <summary>
        /// 订单来源
        /// </summary>
        public OrderSourceType SourceType { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus Status { get; set; }
        /// <summary>
        /// 配送公司
        /// </summary>
        [MaxLength(100)]
        public string? DeliveryCompany { get; set; }
        /// <summary>
        /// 配送单号
        /// </summary>
        [MaxLength(64)]
        public string? DeliverySn { get; set; }
        /// <summary>
        /// 自动确认收货天数
        /// </summary>
        public int AutoConfirmDay { get; set; }
        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Integration { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        public BillType BillType { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        [MaxLength(200)]
        public string? BillHeader { get; set; }
        /// <summary>
        /// 发票内容
        /// </summary>
        [MaxLength(500)]
        public string? BillContent { get; set; }
        /// <summary>
        /// 发票接收手机号
        /// </summary>
        [MaxLength(20)]
        public string? BillReceiverPhone { get; set; }
        /// <summary>
        /// 发票接收邮箱
        /// </summary>
        [MaxLength(100)]
        public string? BillReceiverEmail { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [MaxLength(50)]
        public string? ReceiverName { get; set; }
        /// <summary>
        /// 收货人手机号
        /// </summary>
        [MaxLength(20)]
        public string? ReceiverPhone { get; set; }
        /// <summary>
        /// 收货人邮编
        /// </summary>
        [MaxLength(20)]
        public string? ReceiverPostCode { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        [MaxLength(50)]
        public string? ReceiverProvince { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [MaxLength(50)]
        public string? ReceiverCity { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [MaxLength(50)]
        public string? ReceiverRegion { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(200)]
        public string? ReceiverDetailAddress { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        [MaxLength(500)]
        public string? Note { get; set; }
        /// <summary>
        /// 是否已确认
        /// </summary>
        public bool IsConfirm { get; set; }
        /// <summary>
        /// 使用积分
        /// </summary>
        public int UseIntegration { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PaymentTime { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? DeliveryTime { get; set; }
        /// <summary>
        /// 收货时间
        /// </summary>
        public DateTime? ReceiveTime { get; set; }
        /// <summary>
        /// 评价时间
        /// </summary>
        public DateTime? CommentTime { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
    }
}
