namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 商品属性（属性库）
    /// </summary>
    public class AttrResponse
    {
        public long Id { get; set; }
        /// <summary>所属分类Id</summary>
        public long CategoryId { get; set; }
        /// <summary>属性名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>属性类型（0销售/1基本/2两者）</summary>
        public int AttrType { get; set; }
        /// <summary>可选值（逗号分隔）</summary>
        public string? ValueSelect { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
    }
}
