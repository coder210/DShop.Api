using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 订单流水记录
    /// </summary>
    [Table("OrderRecords")]
    public class OrderRecord : ShopEntityBase
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [MaxLength(64)]
        public string? OrderNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
