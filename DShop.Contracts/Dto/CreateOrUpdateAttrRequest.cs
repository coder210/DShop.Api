namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 新建/更新属性库请求
    /// </summary>
    public class CreateOrUpdateAttrRequest
    {
        /// <summary>主键Id（新建为0）</summary>
        public long Id { get; set; }
        /// <summary>所属分类Id</summary>
        public long CategoryId { get; set; }
        /// <summary>属性名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>属性类型（0销售/1基本/2两者）</summary>
        public int AttrType { get; set; }
        /// <summary>可选值（逗号分隔）</summary>
        public string? ValueSelect { get; set; }
        /// <summary>是否需要检索</summary>
        public int SearchType { get; set; }
        /// <summary>值类型（0单个/1多个）</summary>
        public int ValueType { get; set; }
        /// <summary>是否展示在介绍上</summary>
        public bool ShowDesc { get; set; }
        /// <summary>状态</summary>
        public int Status { get; set; }
    }
}
