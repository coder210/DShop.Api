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
    /// 订单命令服务（Order Command）
    /// </summary>
    public class OrderCommandService : IOrderCommandService
    {
        private readonly DatabaseContext _context;
        private readonly IUserContext _userContext;

        public OrderCommandService(DatabaseContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public (bool Success, string Message) ShipOrder(ShipOrderRequest request)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == request.OrderId && !o.IsDeleted);
            if (order == null)
            {
                return (false, "订单不存在");
            }
            if (order.Status != OrderStatus.PendingShipment)
            {
                return (false, "当前订单状态不允许发货");
            }

            var now = DateTime.Now;
            var operateMan = GetOperateMan();
            order.DeliveryCompany = request.DeliveryCompany;
            order.DeliverySn = request.DeliverySn;
            order.DeliveryTime = now;
            order.Status = OrderStatus.Shipped;
            order.ModifiedBy = _userContext.CurrentUserId;
            order.ModifiedAt = now;

            _context.OrderOperateHistories.Add(new OrderOperateHistory
            {
                OrderId = order.Id,
                OperateMan = operateMan,
                OrderStatus = OrderStatus.Shipped,
                Note = string.IsNullOrWhiteSpace(request.Note) ? "订单发货" : request.Note,
                IsDeleted = false,
                CreatedBy = _userContext.CurrentUserId,
                ModifiedBy = _userContext.CurrentUserId,
                CreatedAt = now,
                ModifiedAt = now
            });

            _context.SaveChanges();
            return (true, "发货成功");
        }

        public (bool Success, string Message) UpdateOrderStatus(UpdateOrderStatusRequest request)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == request.OrderId && !o.IsDeleted);
            if (order == null)
            {
                return (false, "订单不存在");
            }
            if (!Enum.IsDefined(typeof(OrderStatus), request.Status))
            {
                return (false, "无效的状态");
            }

            var now = DateTime.Now;
            var operateMan = GetOperateMan();
            order.Status = (OrderStatus)request.Status;
            order.ModifiedBy = _userContext.CurrentUserId;
            order.ModifiedAt = now;

            _context.OrderOperateHistories.Add(new OrderOperateHistory
            {
                OrderId = order.Id,
                OperateMan = operateMan,
                OrderStatus = (OrderStatus)request.Status,
                Note = request.Note,
                IsDeleted = false,
                CreatedBy = _userContext.CurrentUserId,
                ModifiedBy = _userContext.CurrentUserId,
                CreatedAt = now,
                ModifiedAt = now
            });

            _context.SaveChanges();
            return (true, "操作成功");
        }

        private string GetOperateMan()
        {
            return _userContext.CurrentUserId > 0 ? $"管理员{_userContext.CurrentUserId}" : "系统";
        }
    }
}
