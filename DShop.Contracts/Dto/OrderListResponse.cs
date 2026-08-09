using System;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 订单列表项
    /// </summary>
    public class OrderListResponse
    {
        public long Id { get; set; }
        /// <summary>订单编号</summary>
        public string OrderSn { get; set; } = string.Empty;
        /// <summary>客户Id</summary>
        public long CustomerId { get; set; }
        /// <summary>客户手机号</summary>
        public string? CustomerMobile { get; set; }
        /// <summary>应收金额（分）</summary>
        public int TotalAmount { get; set; }
        /// <summary>实付金额（分）</summary>
        public int PayAmount { get; set; }
        /// <summary>支付方式</summary>
        public int PayType { get; set; }
        /// <summary>订单来源</summary>
        public int SourceType { get; set; }
        /// <summary>订单状态</summary>
        public int Status { get; set; }
        /// <summary>收货人</summary>
        public string? ReceiverName { get; set; }
        /// <summary>收货人手机号</summary>
        public string? ReceiverPhone { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }
}
