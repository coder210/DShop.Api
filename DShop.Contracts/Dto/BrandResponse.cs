namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 商品品牌
    /// </summary>
    public class BrandResponse
    {
        public long Id { get; set; }
        /// <summary>品牌名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Logo</summary>
        public string? Logo { get; set; }
        /// <summary>描述</summary>
        public string? Desc { get; set; }
        /// <summary>首字母</summary>
        public string? FirstLetter { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
        /// <summary>排序</summary>
        public int SortOrder { get; set; }
    }
}
