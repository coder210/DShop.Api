using System.Collections.Generic;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 新建/更新商品SPU请求
    /// </summary>
    public class CreateOrUpdateSpuRequest
    {
        /// <summary>主键Id（新建为0）</summary>
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
        /// <summary>SKU 列表（由前端按规格笛卡尔积生成）</summary>
        public List<CreateSkuRequest> Skus { get; set; } = new List<CreateSkuRequest>();
        /// <summary>商品图片</summary>
        public List<string> Images { get; set; } = new List<string>();
        /// <summary>SPU 属性值（键值对）</summary>
        public List<SpuAttrValueRequest> SpuAttrValues { get; set; } = new List<SpuAttrValueRequest>();
        /// <summary>规格组（用于前端生成笛卡尔积，如 颜色=[红,蓝]）</summary>
        public List<SpecGroupRequest> SpecGroups { get; set; } = new List<SpecGroupRequest>();
    }

    /// <summary>
    /// 新建SKU请求
    /// </summary>
    public class CreateSkuRequest
    {
        /// <summary>主键Id（新建为0）</summary>
        public long Id { get; set; }
        /// <summary>规格图片</summary>
        public string? ImageUrl { get; set; }
        /// <summary>销售价（分）</summary>
        public int Price { get; set; }
        /// <summary>条码</summary>
        public string? BarCode { get; set; }
        /// <summary>二维码</summary>
        public string? QrCode { get; set; }
        /// <summary>该SKU的规格值组合（如 颜色=红, 尺码=S）</summary>
        public List<SkuAttrValueRequest> AttrValues { get; set; } = new List<SkuAttrValueRequest>();
    }

    /// <summary>
    /// SPU属性值请求（键值对）
    /// </summary>
    public class SpuAttrValueRequest
    {
        /// <summary>属性Id（手填为0）</summary>
        public long AttrId { get; set; }
        /// <summary>属性名</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>属性值</summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// 规格组请求（如 颜色=[红,蓝]）
    /// </summary>
    public class SpecGroupRequest
    {
        /// <summary>属性Id（手填为0）</summary>
        public long AttrId { get; set; }
        /// <summary>规格名（如 颜色、尺码）</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>可选值列表（如 红、蓝）</summary>
        public List<string> Values { get; set; } = new List<string>();
    }

    /// <summary>
    /// SKU规格值请求
    /// </summary>
    public class SkuAttrValueRequest
    {
        /// <summary>属性Id（手填为0）</summary>
        public long AttrId { get; set; }
        /// <summary>规格名（如 颜色）</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>规格值（如 红）</summary>
        public string Value { get; set; } = string.Empty;
    }
}
