using System;
using System.Collections.Generic;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 订单详情
    /// </summary>
    public class OrderDetailResponse
    {
        public long Id { get; set; }
        /// <summary>订单编号</summary>
        public string OrderSn { get; set; } = string.Empty;
        /// <summary>客户Id</summary>
        public long CustomerId { get; set; }
        /// <summary>客户手机号</summary>
        public string? CustomerMobile { get; set; }
        /// <summary>商品总额（分）</summary>
        public int TotalAmount { get; set; }
        /// <summary>应付金额（分）</summary>
        public int PayAmount { get; set; }
        /// <summary>运费（分）</summary>
        public int FreightAmount { get; set; }
        /// <summary>促销优惠（分）</summary>
        public int PromotionAmount { get; set; }
        /// <summary>积分抵扣（分）</summary>
        public int IntegrationAmount { get; set; }
        /// <summary>优惠券抵扣（分）</summary>
        public int CouponAmount { get; set; }
        /// <summary>折扣金额（分）</summary>
        public int DiscountAmount { get; set; }
        /// <summary>支付方式</summary>
        public int PayType { get; set; }
        /// <summary>订单来源</summary>
        public int SourceType { get; set; }
        /// <summary>订单状态</summary>
        public int Status { get; set; }
        /// <summary>物流公司</summary>
        public string? DeliveryCompany { get; set; }
        /// <summary>物流单号</summary>
        public string? DeliverySn { get; set; }
        /// <summary>收货人</summary>
        public string? ReceiverName { get; set; }
        /// <summary>收货人手机号</summary>
        public string? ReceiverPhone { get; set; }
        /// <summary>收货地址</summary>
        public string? ReceiverAddress { get; set; }
        /// <summary>订单备注</summary>
        public string? Note { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>订单明细</summary>
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
        /// <summary>操作历史</summary>
        public List<OrderOperateHistoryResponse> Histories { get; set; } = new List<OrderOperateHistoryResponse>();
    }

    /// <summary>
    /// 订单明细
    /// </summary>
    public class OrderItemResponse
    {
        public long Id { get; set; }
        /// <summary>SPU Id</summary>
        public long SpuId { get; set; }
        /// <summary>商品名称</summary>
        public string SpuName { get; set; } = string.Empty;
        /// <summary>SKU Id</summary>
        public long SkuId { get; set; }
        /// <summary>SKU名称</summary>
        public string? SkuName { get; set; }
        /// <summary>SKU图片</summary>
        public string? SkuPic { get; set; }
        /// <summary>SKU售价（分）</summary>
        public int SkuPrice { get; set; }
        /// <summary>SKU数量</summary>
        public int SkuQuantity { get; set; }
        /// <summary>规格值</summary>
        public string? SkuAttrsVals { get; set; }
        /// <summary>实付金额（分）</summary>
        public int RealAmount { get; set; }
    }

    /// <summary>
    /// 订单操作历史
    /// </summary>
    public class OrderOperateHistoryResponse
    {
        public long Id { get; set; }
        /// <summary>操作人</summary>
        public string OperateMan { get; set; } = string.Empty;
        /// <summary>操作后状态</summary>
        public int OrderStatus { get; set; }
        /// <summary>备注</summary>
        public string? Note { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }
}
