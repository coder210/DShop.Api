using System.ComponentModel.DataAnnotations;

namespace DShop.Models
{
    /// <summary>
    /// 电商领域实体基类，实现统一的溯源字段（ITraceable）。
    /// </summary>
    public abstract class ShopEntityBase : ITraceable
    {
        /// <summary>主键Id</summary>
        [Key]
        public long Id { get; set; }
        /// <summary>是否删除</summary>
        public bool IsDeleted { get; set; }
        /// <summary>最后修改人Id</summary>
        public long ModifiedBy { get; set; }
        /// <summary>最后修改时间</summary>
        public DateTime ModifiedAt { get; set; }
        /// <summary>创建人Id</summary>
        public long CreatedBy { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }
}
