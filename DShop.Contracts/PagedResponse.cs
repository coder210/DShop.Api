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
        /// <summary>总记录数（TotalCount 的别名，便于前端兼容）</summary>
        public int Total { get { return TotalCount; } set { TotalCount = value; } }
        /// <summary>当前页码（PageIndex 的别名，便于前端兼容）</summary>
        public int Page { get { return PageIndex; } set { PageIndex = value; } }
        /// <summary>每页大小（PageSize 的别名，便于前端兼容）</summary>
        public int Size { get { return PageSize; } set { PageSize = value; } }
    }
}
