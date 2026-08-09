using System.Collections.Generic;
using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 订单查询服务（Order Query）
    /// </summary>
    public interface IOrderQueryService
    {
        /// <summary>
        /// 订单分页列表
        /// </summary>
        PagedResponse<OrderListResponse> GetOrderList(string? keyword, int status, int pageIndex, int pageSize);

        /// <summary>
        /// 订单详情（含明细、操作历史）
        /// </summary>
        OrderDetailResponse? GetOrderDetail(long id, out string msg);
    }
}
