using System;
using System.Linq;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.Models;

namespace DShop.AdminPlugin.Services
{
    /// <summary>
    /// 售后查询服务
    /// </summary>
    public class RefundQueryService : IRefundQueryService
    {
        private readonly DatabaseContext _context;

        public RefundQueryService(DatabaseContext context)
        {
            _context = context;
        }

        public PagedResponse<RefundOrderListResponse> GetRefundList(string? keyword, int status, int pageIndex, int pageSize)
        {
            var query = _context.RefundOrders.Where(r => !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(r => r.OrderSn.Contains(keyword) ||
                    (r.CustomerMobile != null && r.CustomerMobile.Contains(keyword)));
            }
            if (status >= 0)
            {
                var enumStatus = (RefundStatus)status;
                if (Enum.IsDefined(typeof(RefundStatus), enumStatus))
                {
                    query = query.Where(r => r.Status == enumStatus);
                }
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RefundOrderListResponse
                {
                    Id = r.Id,
                    OrderId = r.OrderId,
                    OrderSn = r.OrderSn,
                    CustomerMobile = r.CustomerMobile,
                    RefundType = (int)r.RefundType,
                    Reason = r.Reason,
                    RefundAmount = r.RefundAmount,
                    Status = (int)r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            return new PagedResponse<RefundOrderListResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public RefundOrderDetailResponse? GetRefundDetail(long id, out string msg)
        {
            var refund = _context.RefundOrders.FirstOrDefault(r => r.Id == id && !r.IsDeleted);
            if (refund == null)
            {
                msg = "退款单不存在";
                return null;
            }

            msg = "获取成功";
            return new RefundOrderDetailResponse
            {
                Id = refund.Id,
                OrderId = refund.OrderId,
                OrderSn = refund.OrderSn,
                CustomerId = refund.CustomerId,
                CustomerMobile = refund.CustomerMobile,
                RefundType = (int)refund.RefundType,
                Reason = refund.Reason,
                RefundAmount = refund.RefundAmount,
                Status = (int)refund.Status,
                AuditorName = refund.AuditorName,
                AuditTime = refund.AuditTime,
                AuditRemark = refund.AuditRemark,
                RefundTime = refund.RefundTime,
                CreatedAt = refund.CreatedAt
            };
        }
    }
}
