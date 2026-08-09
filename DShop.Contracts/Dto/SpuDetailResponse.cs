using System;
using System.Collections.Generic;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 商品SPU详情
    /// </summary>
    public class SpuDetailResponse
    {
        public long Id { get; set; }
        /// <summary>商品名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>分类Id</summary>
        public long CategoryId { get; set; }
        /// <summary>品牌Id</summary>
        public long BrandId { get; set; }
        /// <summary>重量（克）</summary>
        public decimal Weight { get; set; }
        /// <summary>商品描述</summary>
        public string? Desc { get; set; }
        /// <summary>商品状态</summary>
        public int Status { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>SKU 列表</summary>
        public List<SkuResponse> Skus { get; set; } = new List<SkuResponse>();
        /// <summary>商品图片</summary>
        public List<string> Images { get; set; } = new List<string>();
        /// <summary>SPU 属性值（键值对）</summary>
        public List<SpuAttrValueResponse> SpuAttrValues { get; set; } = new List<SpuAttrValueResponse>();
        /// <summary>规格组</summary>
        public List<SpecGroupResponse> SpecGroups { get; set; } = new List<SpecGroupResponse>();
    }

    /// <summary>
    /// SPU属性值
    /// </summary>
    public class SpuAttrValueResponse
    {
        public long AttrId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// 规格组
    /// </summary>
    public class SpecGroupResponse
    {
        public long AttrId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new List<string>();
    }
}
