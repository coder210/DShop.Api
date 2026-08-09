namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 审核售后/退款请求
    /// </summary>
    public class AuditRefundRequest
    {
        /// <summary>退款单Id</summary>
        public long Id { get; set; }
        /// <summary>审核结果：true=同意，false=拒绝</summary>
        public bool Agree { get; set; }
        /// <summary>审核备注</summary>
        public string? Remark { get; set; }
    }
}
