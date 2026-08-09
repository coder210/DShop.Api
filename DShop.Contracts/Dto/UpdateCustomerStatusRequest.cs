namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 更新客户状态请求
    /// </summary>
    public class UpdateCustomerStatusRequest
    {
        /// <summary>客户Id</summary>
        public long Id { get; set; }
        /// <summary>状态（Enable/Disable）</summary>
        public int Status { get; set; }
    }
}
