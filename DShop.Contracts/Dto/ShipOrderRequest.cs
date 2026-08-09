namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 订单发货请求
    /// </summary>
    public class ShipOrderRequest
    {
        /// <summary>订单Id</summary>
        public long OrderId { get; set; }
        /// <summary>物流公司</summary>
        public string? DeliveryCompany { get; set; }
        /// <summary>物流单号</summary>
        public string? DeliverySn { get; set; }
        /// <summary>备注</summary>
        public string? Note { get; set; }
    }
}
