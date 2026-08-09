using System.Collections.Generic;

namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 商品分类树节点
    /// </summary>
    public class CategoryTreeResponse
    {
        public long Id { get; set; }
        /// <summary>父分类Id</summary>
        public long ParentId { get; set; }
        /// <summary>分类名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>图标</summary>
        public string? Icon { get; set; }
        /// <summary>层级</summary>
        public int Level { get; set; }
        /// <summary>排序</summary>
        public int SortOrder { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
        /// <summary>子分类</summary>
        public List<CategoryTreeResponse> Children { get; set; } = new List<CategoryTreeResponse>();
    }
}
