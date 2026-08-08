namespace DShop.AdminPlugin.Responses
{
    public class CategoryResponse
    {

        public int Id { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string ModelType { get; set; }
        /// <summary>
        /// 品牌厂家
        /// </summary>
        public string BrandManufacturer { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 工作方式
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateAt { get; set; }
    }
}
