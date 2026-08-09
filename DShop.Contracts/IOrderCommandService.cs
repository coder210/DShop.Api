using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    /// <summary>
    /// 订单命令服务（Order Command）
    /// </summary>
    public interface IOrderCommandService
    {
        /// <summary>
        /// 订单发货
        /// </summary>
        (bool Success, string Message) ShipOrder(ShipOrderRequest request);

        /// <summary>
        /// 更新订单状态（如关闭订单）
        /// </summary>
        (bool Success, string Message) UpdateOrderStatus(UpdateOrderStatusRequest request);
    }
}
