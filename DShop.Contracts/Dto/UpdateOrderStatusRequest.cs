namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 更新订单状态请求（关闭等）
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        /// <summary>订单Id</summary>
        public long OrderId { get; set; }
        /// <summary>目标状态</summary>
        public int Status { get; set; }
        /// <summary>备注</summary>
        public string? Note { get; set; }
    }
}
