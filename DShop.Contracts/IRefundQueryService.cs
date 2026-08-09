using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 售后查询服务（Refund Query）
    /// </summary>
    public interface IRefundQueryService
    {
        /// <summary>
        /// 售后/退款单分页列表
        /// </summary>
        PagedResponse<RefundOrderListResponse> GetRefundList(string? keyword, int status, int pageIndex, int pageSize);

        /// <summary>
        /// 售后/退款单详情
        /// </summary>
        RefundOrderDetailResponse? GetRefundDetail(long id, out string msg);
    }
}
