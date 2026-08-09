using System;
using System.Collections.Generic;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 首页看板总览
    /// </summary>
    public class DashboardOverviewResponse
    {
        /// <summary>今日订单数</summary>
        public int TodayOrderCount { get; set; }
        /// <summary>今日销售额（分）</summary>
        public long TodaySalesAmount { get; set; }
        /// <summary>累计订单数</summary>
        public int TotalOrderCount { get; set; }
        /// <summary>累计销售额（分）</summary>
        public long TotalSalesAmount { get; set; }
        /// <summary>待发货订单数</summary>
        public int PendingShipmentCount { get; set; }
        /// <summary>待付款订单数</summary>
        public int PendingPaymentCount { get; set; }
        /// <summary>商品总数</summary>
        public int ProductCount { get; set; }
        /// <summary>客户总数</summary>
        public int CustomerCount { get; set; }
        /// <summary>近7天销售趋势</summary>
        public List<DailySalesResponse> Trend { get; set; } = new List<DailySalesResponse>();
    }

    /// <summary>
    /// 每日销售额
    /// </summary>
    public class DailySalesResponse
    {
        /// <summary>日期（yyyy-MM-dd）</summary>
        public string Date { get; set; } = string.Empty;
        /// <summary>订单数</summary>
        public int OrderCount { get; set; }
        /// <summary>销售额（分）</summary>
        public long SalesAmount { get; set; }
    }
}
