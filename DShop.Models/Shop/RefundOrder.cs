using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 退款类型
    /// </summary>
    public enum RefundType
    {
        /// <summary>仅退款</summary>
        OnlyMoney,
        /// <summary>退货退款</summary>
        ReturnGoods
    }

    /// <summary>
    /// 退款状态
    /// </summary>
    public enum RefundStatus
    {
        /// <summary>待审核</summary>
        Pending,
        /// <summary>已同意</summary>
        Agreed,
        /// <summary>已拒绝</summary>
        Rejected,
        /// <summary>退款中</summary>
        Processing,
        /// <summary>已退款</summary>
        Refunded,
        /// <summary>已关闭</summary>
        Closed
    }

    /// <summary>
    /// 售后/退款单
    /// </summary>
    [Table("RefundOrders")]
    public class RefundOrder : ShopEntityBase
    {
        /// <summary>所属订单Id</summary>
        public long OrderId { get; set; }
        /// <summary>订单编号</summary>
        [MaxLength(64)]
        public string OrderSn { get; set; } = string.Empty;
        /// <summary>客户Id</summary>
        public long CustomerId { get; set; }
        /// <summary>客户手机号</summary>
        [MaxLength(20)]
        public string? CustomerMobile { get; set; }
        /// <summary>退款类型</summary>
        public RefundType RefundType { get; set; }
        /// <summary>退款原因</summary>
        [MaxLength(500)]
        public string? Reason { get; set; }
        /// <summary>退款金额（分）</summary>
        public int RefundAmount { get; set; }
        /// <summary>退款状态</summary>
        public RefundStatus Status { get; set; }
        /// <summary>审核人Id</summary>
        public long AuditorId { get; set; }
        /// <summary>审核人</summary>
        [MaxLength(50)]
        public string? AuditorName { get; set; }
        /// <summary>审核时间</summary>
        public DateTime? AuditTime { get; set; }
        /// <summary>审核备注</summary>
        [MaxLength(500)]
        public string? AuditRemark { get; set; }
        /// <summary>退款完成时间</summary>
        public DateTime? RefundTime { get; set; }
    }
}
