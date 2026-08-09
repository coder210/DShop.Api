using System;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 积分流水
    /// </summary>
    public class CoinRecordResponse
    {
        public long Id { get; set; }
        /// <summary>增减类型（Increase/Subtract）</summary>
        public int Type { get; set; }
        /// <summary>标题</summary>
        public string? Title { get; set; }
        /// <summary>金额（积分）</summary>
        public int Amount { get; set; }
        /// <summary>备注</summary>
        public string? Remark { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }
}
