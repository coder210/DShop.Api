using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 首页看板查询服务（Dashboard）
    /// </summary>
    public interface IDashboardQueryService
    {
        /// <summary>
        /// 获取首页看板总览数据
        /// </summary>
        DashboardOverviewResponse GetOverview();
    }
}
