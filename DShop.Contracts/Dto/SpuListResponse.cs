using System;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 商品SPU列表项
    /// </summary>
    public class SpuListResponse
    {
        public long Id { get; set; }
        /// <summary>商品名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>分类Id</summary>
        public long CategoryId { get; set; }
        /// <summary>分类名称</summary>
        public string? CategoryName { get; set; }
        /// <summary>品牌Id</summary>
        public long BrandId { get; set; }
        /// <summary>品牌名称</summary>
        public string? BrandName { get; set; }
        /// <summary>商品状态（PutOnShelves/PutOffShelves）</summary>
        public int Status { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }
}
