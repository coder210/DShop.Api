using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 售后命令服务（Refund Command）
    /// </summary>
    public interface IRefundCommandService
    {
        /// <summary>
        /// 审核退款（同意/拒绝）
        /// </summary>
        (bool Success, string Message) AuditRefund(AuditRefundRequest request);
    }
}
