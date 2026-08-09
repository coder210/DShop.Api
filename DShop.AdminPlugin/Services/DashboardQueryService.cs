using System;
using System.Collections.Generic;
using System.Linq;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.Models;

namespace DShop.AdminPlugin.Services
{
    /// <summary>
    /// 首页看板查询服务
    /// </summary>
    public class DashboardQueryService : IDashboardQueryService
    {
        private readonly DatabaseContext _context;

        public DashboardQueryService(DatabaseContext context)
        {
            _context = context;
        }

        public DashboardOverviewResponse GetOverview()
        {
            var now = DateTime.Now;
            var todayStart = now.Date;

            // 有效订单（未删除、非无效订单）
            var validOrders = _context.Orders.Where(o => !o.IsDeleted && o.Status != OrderStatus.InvalidOrder).ToList();
            var todayOrders = validOrders.Where(o => o.CreatedAt >= todayStart).ToList();

            var validStatuses = new[] { OrderStatus.PendingPayment, OrderStatus.PendingShipment, OrderStatus.Shipped, OrderStatus.PendingEvaluation, OrderStatus.Finished };

            var todaySales = todayOrders
                .Where(o => validStatuses.Contains(o.Status))
                .Sum(o => (long)o.PayAmount);
            var totalSales = validOrders
                .Where(o => validStatuses.Contains(o.Status))
                .Sum(o => (long)o.PayAmount);

            var pendingShipment = validOrders.Count(o => o.Status == OrderStatus.PendingShipment);
            var pendingPayment = validOrders.Count(o => o.Status == OrderStatus.PendingPayment);

            var productCount = _context.Spus.Count(s => !s.IsDeleted);
            var customerCount = _context.Customers.Count(c => !c.IsDeleted);

            // 近7天销售趋势
            var trend = new List<DailySalesResponse>();
            for (int i = 6; i >= 0; i--)
            {
                var day = now.Date.AddDays(-i);
                var dayOrders = validOrders.Where(o => o.CreatedAt >= day && o.CreatedAt < day.AddDays(1)).ToList();
                trend.Add(new DailySalesResponse
                {
                    Date = day.ToString("yyyy-MM-dd"),
                    OrderCount = dayOrders.Count,
                    SalesAmount = dayOrders.Where(o => validStatuses.Contains(o.Status)).Sum(o => (long)o.PayAmount)
                });
            }

            return new DashboardOverviewResponse
            {
                TodayOrderCount = todayOrders.Count,
                TodaySalesAmount = todaySales,
                TotalOrderCount = validOrders.Count,
                TotalSalesAmount = totalSales,
                PendingShipmentCount = pendingShipment,
                PendingPaymentCount = pendingPayment,
                ProductCount = productCount,
                CustomerCount = customerCount,
                Trend = trend
            };
        }
    }
}
