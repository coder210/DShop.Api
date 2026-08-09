using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 验证码
    /// </summary>
    [Table("IdentifyingCodes")]
    public class IdentifyingCode : ShopEntityBase
    {
        /// <summary>
        /// 验证码类型
        /// </summary>
        public IdentifyingCodeType Type { get; set; }
        /// <summary>
        /// 区号
        /// </summary>
        [MaxLength(10)]
        public string? AreaCode { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string? Mobile { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [MaxLength(20)]
        public string? Code { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public IdentifyingCodeStatus Status { get; set; }
    }
}
