using System;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 客户详情
    /// </summary>
    public class CustomerDetailResponse
    {
        public long Id { get; set; }
        /// <summary>手机号</summary>
        public string? Mobile { get; set; }
        /// <summary>昵称</summary>
        public string? Nickname { get; set; }
        /// <summary>邮箱</summary>
        public string? Email { get; set; }
        /// <summary>签名</summary>
        public string? Idiograph { get; set; }
        /// <summary>积分余额</summary>
        public int Coin { get; set; }
        /// <summary>性别</summary>
        public int Gender { get; set; }
        /// <summary>头像</summary>
        public string? Avatar { get; set; }
        /// <summary>地址</summary>
        public string? Address { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>地址数量</summary>
        public int AddressCount { get; set; }
        /// <summary>订单数量</summary>
        public int OrderCount { get; set; }
        /// <summary>收藏数量</summary>
        public int CollectCount { get; set; }
    }
}
