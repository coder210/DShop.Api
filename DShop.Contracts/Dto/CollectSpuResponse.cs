using System;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 客户收藏
    /// </summary>
    public class CollectSpuResponse
    {
        public long Id { get; set; }
        /// <summary>商品SPU Id</summary>
        public long SpuId { get; set; }
        /// <summary>商品名称</summary>
        public string? SpuName { get; set; }
        /// <summary>商品价格（分）</summary>
        public int SpuPrice { get; set; }
        /// <summary>商品图片</summary>
        public string? SpuImageUrl { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }
}
