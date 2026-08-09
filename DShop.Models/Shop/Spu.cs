using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 商品SPU（标准产品单元）
    /// </summary>
    [Table("Spus")]
    public class Spu : ShopEntityBase
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 分类Id
        /// </summary>
        public long CategoryId { get; set; }
        /// <summary>
        /// 品牌Id
        /// </summary>
        public long BrandId { get; set; }
        /// <summary>
        /// 重量（克）
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string? Desc { get; set; }
        /// <summary>
        /// 商品状态
        /// </summary>
        public SpuStatus Status { get; set; }
    }
}
