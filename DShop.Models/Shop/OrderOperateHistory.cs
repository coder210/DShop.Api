using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 订单操作历史
    /// </summary>
    [Table("OrderOperateHistories")]
    public class OrderOperateHistory : ShopEntityBase
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        [MaxLength(50)]
        public string OperateMan { get; set; } = string.Empty;
        /// <summary>
        /// 操作后订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
