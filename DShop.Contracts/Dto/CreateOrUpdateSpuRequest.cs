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
        /// <summary>SKU 列表</summary>
        public List<CreateSkuRequest> Skus { get; set; } = new List<CreateSkuRequest>();
        /// <summary>商品图片</summary>
        public List<string> Images { get; set; } = new List<string>();
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
    }
}
