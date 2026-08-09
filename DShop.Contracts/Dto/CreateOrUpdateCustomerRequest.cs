namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 新建/更新客户请求
    /// </summary>
    public class CreateOrUpdateCustomerRequest
    {
        /// <summary>主键Id（新建为0）</summary>
        public long Id { get; set; }
        /// <summary>手机号（必填）</summary>
        public string Mobile { get; set; } = string.Empty;
        /// <summary>昵称</summary>
        public string? Nickname { get; set; }
        /// <summary>邮箱</summary>
        public string? Email { get; set; }
        /// <summary>密码（仅新建或重置时传入）</summary>
        public string? Password { get; set; }
        /// <summary>性别</summary>
        public int Gender { get; set; }
        /// <summary>头像</summary>
        public string? Avatar { get; set; }
        /// <summary>地址</summary>
        public string? Address { get; set; }
        /// <summary>积分</summary>
        public int Coin { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
    }
}
