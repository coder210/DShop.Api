using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    [Table("Users")]
    public class User : ITraceable
    {
        [Key]
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string MobilePhoneNumber { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 密码hash
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// 最后一次登出时间
        /// </summary>
        public DateTime LastLoginAt { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        [Required]
        /// <summary>
        /// 最后一个修改人的id
        /// </summary>
        public long ModifiedBy { get; set; }
        [Required]
        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        public DateTime ModifiedAt { get; set; }
        [Required]
        /// <summary>
        /// 创建人的id
        /// </summary>
        public long CreatedBy { get; set; }
        [Required]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
