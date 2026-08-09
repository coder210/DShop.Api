namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 新建/更新品牌请求
    /// </summary>
    public class CreateOrUpdateBrandRequest
    {
        /// <summary>主键Id（新建为0）</summary>
        public long Id { get; set; }
        /// <summary>品牌名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Logo</summary>
        public string? Logo { get; set; }
        /// <summary>描述</summary>
        public string? Desc { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
        /// <summary>排序</summary>
        public int SortOrder { get; set; }
    }
}
