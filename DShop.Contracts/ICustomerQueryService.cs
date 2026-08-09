using System.Collections.Generic;
using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 客户查询服务（Customer Query）
    /// </summary>
    public interface ICustomerQueryService
    {
        /// <summary>
        /// 客户分页列表
        /// </summary>
        PagedResponse<CustomerListResponse> GetCustomerList(string? keyword, int pageIndex, int pageSize);

        /// <summary>
        /// 客户详情
        /// </summary>
        CustomerDetailResponse? GetCustomerDetail(long id, out string msg);

        /// <summary>
        /// 客户收货地址列表
        /// </summary>
        List<DeliveryAddressResponse> GetCustomerAddresses(long customerId);

        /// <summary>
        /// 客户积分流水
        /// </summary>
        PagedResponse<CoinRecordResponse> GetCoinRecords(long customerId, int pageIndex, int pageSize);

        /// <summary>
        /// 客户浏览记录
        /// </summary>
        PagedResponse<BrowsingSpuResponse> GetBrowsingSpus(long customerId, int pageIndex, int pageSize);

        /// <summary>
        /// 客户收藏列表
        /// </summary>
        PagedResponse<CollectSpuResponse> GetCollectSpus(long customerId, int pageIndex, int pageSize);
    }
}
