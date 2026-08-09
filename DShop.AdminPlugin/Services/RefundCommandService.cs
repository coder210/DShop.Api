using System;
using System.Linq;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.Models;
using DShop.PluginShared;

namespace DShop.AdminPlugin.Services
{
    /// <summary>
    /// 售后命令服务
    /// </summary>
    public class RefundCommandService : IRefundCommandService
    {
        private readonly DatabaseContext _context;
        private readonly IUserContext _userContext;

        public RefundCommandService(DatabaseContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public (bool Success, string Message) AuditRefund(AuditRefundRequest request)
        {
            var refund = _context.RefundOrders.FirstOrDefault(r => r.Id == request.Id && !r.IsDeleted);
            if (refund == null)
            {
                return (false, "退款单不存在");
            }
            if (refund.Status != RefundStatus.Pending)
            {
                return (false, "当前状态不允许审核");
            }

            var now = DateTime.Now;
            var userId = _userContext.CurrentUserId;

            if (request.Agree)
            {
                refund.Status = RefundStatus.Agreed;
                refund.AuditorId = userId;
                refund.AuditorName = $"管理员{userId}";
                refund.AuditTime = now;
                refund.AuditRemark = request.Remark ?? "同意退款";
            }
            else
            {
                refund.Status = RefundStatus.Rejected;
                refund.AuditorId = userId;
                refund.AuditorName = $"管理员{userId}";
                refund.AuditTime = now;
                refund.AuditRemark = request.Remark ?? "拒绝退款";
            }

            refund.ModifiedBy = userId;
            refund.ModifiedAt = now;
            _context.SaveChanges();
            return (true, request.Agree ? "已同意退款" : "已拒绝退款");
        }
    }
}
