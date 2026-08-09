using System.Collections.Generic;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 商品SKU
    /// </summary>
    public class SkuResponse
    {
        public long Id { get; set; }
        /// <summary>所属SPU Id</summary>
        public long SpuId { get; set; }
        /// <summary>规格图片</summary>
        public string? ImageUrl { get; set; }
        /// <summary>销售价（分）</summary>
        public int Price { get; set; }
        /// <summary>销量</summary>
        public int SaleCount { get; set; }
        /// <summary>条码</summary>
        public string? BarCode { get; set; }
        /// <summary>二维码</summary>
        public string? QrCode { get; set; }
        /// <summary>该SKU的规格值组合</summary>
        public List<SpuAttrValueResponse> AttrValues { get; set; } = new List<SpuAttrValueResponse>();
    }
}
