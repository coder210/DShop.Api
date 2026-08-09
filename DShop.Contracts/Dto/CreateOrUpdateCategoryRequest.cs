namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 新建/更新商品分类请求
    /// </summary>
    public class CreateOrUpdateCategoryRequest
    {
        /// <summary>主键Id（新建为0）</summary>
        public long Id { get; set; }
        /// <summary>父分类Id（0为顶级）</summary>
        public long ParentId { get; set; }
        /// <summary>分类名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>图标</summary>
        public string? Icon { get; set; }
        /// <summary>排序</summary>
        public int SortOrder { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
    }
}
