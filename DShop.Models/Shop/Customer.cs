using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 客户（C端用户）
    /// </summary>
    [Table("Customers")]
    public class Customer : ShopEntityBase
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string? Mobile { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [MaxLength(50)]
        public string? Nickname { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(100)]
        public string? Email { get; set; }
        /// <summary>
        /// 密码哈希
        /// </summary>
        [MaxLength(100)]
        public string? Password { get; set; }
        /// <summary>
        /// 密码盐
        /// </summary>
        [MaxLength(100)]
        public string? Salt { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        [MaxLength(200)]
        public string? Idiograph { get; set; }
        /// <summary>
        /// 积分余额
        /// </summary>
        public int Coin { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public CustomerGender Gender { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [MaxLength(500)]
        public string? Avatar { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(200)]
        public string? Address { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public CustomerStatus Status { get; set; }
    }
}
