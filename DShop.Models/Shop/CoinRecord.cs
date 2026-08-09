using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 客户积分流水
    /// </summary>
    [Table("CoinRecords")]
    public class CoinRecord : ShopEntityBase
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public long CustomerId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string? Mobile { get; set; }
        /// <summary>
        /// 增减类型
        /// </summary>
        public CoinRecordType Type { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(100)]
        public string? Title { get; set; }
        /// <summary>
        /// 金额（积分）
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string? Remark { get; set; }
    }
}
