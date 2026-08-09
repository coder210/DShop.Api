using System;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 客户列表项
    /// </summary>
    public class CustomerListResponse
    {
        public long Id { get; set; }
        /// <summary>手机号</summary>
        public string? Mobile { get; set; }
        /// <summary>昵称</summary>
        public string? Nickname { get; set; }
        /// <summary>邮箱</summary>
        public string? Email { get; set; }
        /// <summary>积分余额</summary>
        public int Coin { get; set; }
        /// <summary>性别</summary>
        public int Gender { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }
}
