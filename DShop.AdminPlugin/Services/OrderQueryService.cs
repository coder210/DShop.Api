using System;
using System.Linq;
using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.Models;

namespace DShop.AdminPlugin.Services
{
    /// <summary>
    /// 订单查询服务（Order Query）
    /// </summary>
    public class OrderQueryService : IOrderQueryService
    {
        private readonly DatabaseContext _context;

        public OrderQueryService(DatabaseContext context)
        {
            _context = context;
        }

        public PagedResponse<OrderListResponse> GetOrderList(string? keyword, int status, int pageIndex, int pageSize)
        {
            var query = _context.Orders.Where(o => !o.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(o => o.OrderSn.Contains(keyword) ||
                    (o.CustomerMobile != null && o.CustomerMobile.Contains(keyword)) ||
                    (o.ReceiverName != null && o.ReceiverName.Contains(keyword)) ||
                    (o.ReceiverPhone != null && o.ReceiverPhone.Contains(keyword)));
            }
            if (status >= 0)
            {
                var enumStatus = (OrderStatus)status;
                if (Enum.IsDefined(typeof(OrderStatus), enumStatus))
                {
                    query = query.Where(o => o.Status == enumStatus);
                }
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderListResponse
                {
                    Id = o.Id,
                    OrderSn = o.OrderSn,
                    CustomerId = o.CustomerId,
                    CustomerMobile = o.CustomerMobile,
                    TotalAmount = o.TotalAmount,
                    PayAmount = o.PayAmount,
                    PayType = (int)o.PayType,
                    SourceType = (int)o.SourceType,
                    Status = (int)o.Status,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    CreatedAt = o.CreatedAt
                })
                .ToList();

            return new PagedResponse<OrderListResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public OrderDetailResponse? GetOrderDetail(long id, out string msg)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id && !o.IsDeleted);
            if (order == null)
            {
                msg = "订单不存在";
                return null;
            }

            var items = _context.OrderItems
                .Where(i => i.OrderId == id && !i.IsDeleted)
                .OrderBy(i => i.Id)
                .Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    SpuId = i.SpuId,
                    SpuName = i.SpuName,
                    SkuId = i.SkuId,
                    SkuName = i.SkuName,
                    SkuPic = i.SkuPic,
                    SkuPrice = i.SkuPrice,
                    SkuQuantity = i.SkuQuantity,
                    SkuAttrsVals = i.SkuAttrsVals,
                    RealAmount = i.RealAmount
                })
                .ToList();

            var histories = _context.OrderOperateHistories
                .Where(h => h.OrderId == id && !h.IsDeleted)
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new OrderOperateHistoryResponse
                {
                    Id = h.Id,
                    OperateMan = h.OperateMan,
                    OrderStatus = (int)h.OrderStatus,
                    Note = h.Note,
                    CreatedAt = h.CreatedAt
                })
                .ToList();

            var receiverAddress = string.Join(" ",
                new[] { order.ReceiverProvince, order.ReceiverCity, order.ReceiverRegion, order.ReceiverDetailAddress }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

            msg = "获取成功";
            return new OrderDetailResponse
            {
                Id = order.Id,
                OrderSn = order.OrderSn,
                CustomerId = order.CustomerId,
                CustomerMobile = order.CustomerMobile,
                TotalAmount = order.TotalAmount,
                PayAmount = order.PayAmount,
                FreightAmount = order.FreightAmount,
                PromotionAmount = order.PromotionAmount,
                IntegrationAmount = order.IntegrationAmount,
                CouponAmount = order.CouponAmount,
                DiscountAmount = order.DiscountAmount,
                PayType = (int)order.PayType,
                SourceType = (int)order.SourceType,
                Status = (int)order.Status,
                DeliveryCompany = order.DeliveryCompany,
                DeliverySn = order.DeliverySn,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ReceiverAddress = receiverAddress,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                Items = items,
                Histories = histories
            };
        }
    }
}
