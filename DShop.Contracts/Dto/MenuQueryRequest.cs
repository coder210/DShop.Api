namespace DShop.Contracts.Dto
{
    public class MenuQueryRequest
    {
        public string Name { get; set; }          // 模糊匹配名称
        public long? ParentId { get; set; }        // 按父级筛选
        public string SortBy { get; set; } // 排序字段
        public bool IsDescending { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
