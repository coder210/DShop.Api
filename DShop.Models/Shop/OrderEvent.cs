using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 订单事件
    /// </summary>
    [Table("OrderEvents")]
    public class OrderEvent : ShopEntityBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public OrderEventType EventType { get; set; }
        /// <summary>
        /// 次数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 最大次数
        /// </summary>
        public int MaxCount { get; set; }
        /// <summary>
        /// 事件状态
        /// </summary>
        public OrderEventStatus Status { get; set; }
        /// <summary>
        /// 事件内容
        /// </summary>
        public string? Content { get; set; }
    }
}
