using System.ComponentModel.DataAnnotations;

namespace DShop.BasePlugin.Requests
{
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// 旧密码(站内修改时必填)
        /// </summary>
        [Required]
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码(站内修改时必填 )
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string? Captcha { get; set; }
    }
}
