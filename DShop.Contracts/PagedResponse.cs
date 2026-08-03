namespace DShop.Contracts
{
    public class PagedResponse<T>
    {
        /// <summary>总记录数</summary>
        public int TotalCount { get; set; }
        /// <summary>当前页码（从1开始）</summary>
        public int PageIndex { get; set; }
        /// <summary>每页大小</summary>
        public int PageSize { get; set; }
        /// <summary>当前页数据</summary>
        public List<T> Items { get; set; }
    }
}
