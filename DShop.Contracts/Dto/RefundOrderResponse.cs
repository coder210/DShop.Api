using System;
using System.Collections.Generic;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 售后/退款单列表项
    /// </summary>
    public class RefundOrderListResponse
    {
        public long Id { get; set; }
        /// <summary>订单Id</summary>
        public long OrderId { get; set; }
        /// <summary>订单编号</summary>
        public string OrderSn { get; set; } = string.Empty;
        /// <summary>客户手机号</summary>
        public string? CustomerMobile { get; set; }
        /// <summary>退款类型</summary>
        public int RefundType { get; set; }
        /// <summary>退款原因</summary>
        public string? Reason { get; set; }
        /// <summary>退款金额（分）</summary>
        public int RefundAmount { get; set; }
        /// <summary>退款状态</summary>
        public int Status { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 售后/退款单详情
    /// </summary>
    public class RefundOrderDetailResponse
    {
        public long Id { get; set; }
        /// <summary>订单Id</summary>
        public long OrderId { get; set; }
        /// <summary>订单编号</summary>
        public string OrderSn { get; set; } = string.Empty;
        /// <summary>客户Id</summary>
        public long CustomerId { get; set; }
        /// <summary>客户手机号</summary>
        public string? CustomerMobile { get; set; }
        /// <summary>退款类型</summary>
        public int RefundType { get; set; }
        /// <summary>退款原因</summary>
        public string? Reason { get; set; }
        /// <summary>退款金额（分）</summary>
        public int RefundAmount { get; set; }
        /// <summary>退款状态</summary>
        public int Status { get; set; }
        /// <summary>审核人</summary>
        public string? AuditorName { get; set; }
        /// <summary>审核时间</summary>
        public DateTime? AuditTime { get; set; }
        /// <summary>审核备注</summary>
        public string? AuditRemark { get; set; }
        /// <summary>退款完成时间</summary>
        public DateTime? RefundTime { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>关联订单详情</summary>
        public OrderDetailResponse? Order { get; set; }
    }
}
