using System.ComponentModel.DataAnnotations;

namespace DShop.AdminPlugin.Requests
{
    public class CategoryUpdateRequest
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 型号/规格/等级
        /// model or specifications or grade
        /// </summary>
        [Required]
        public string ModelType { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [Required]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 品牌厂家 
        /// </summary>
        public string? BrandManufacturer { get; set; }

        /// <summary>
        /// 工作方式
        /// </summary>
        public string? Mode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
