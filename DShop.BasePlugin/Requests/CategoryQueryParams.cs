using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DShop.BasePlugin.Requests
{
    /// <summary>
    /// 报价单匹配信息分页查询参数
    /// </summary>
    public class CategoryQueryParams
    {
        /// <summary>
        /// 当前页码（从1开始）
        /// </summary>
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        [Range(10, 100)]
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 设备名称
        /// </summary>
        [JsonPropertyName("name")]
        public string?Name { get; set; }

        /// <summary>
        /// 型号规格
        /// </summary>
        [JsonPropertyName("modelType")]
        public string? ModelType { get; set; }

        /// <summary>
        /// 最小单价
        /// </summary>
        [JsonPropertyName("minPrice")]
        public int? MinPrice { get; set; }

        /// <summary>
        /// 最大单价
        /// </summary>
        [JsonPropertyName("maxPrice")]
        public int? MaxPrice { get; set; }
    }
}
