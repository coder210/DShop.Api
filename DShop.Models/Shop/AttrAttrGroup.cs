using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 属性与属性分组关联表
    /// </summary>
    [Table("AttrAttrGroups")]
    public class AttrAttrGroup : ShopEntityBase
    {
        /// <summary>
        /// 属性Id
        /// </summary>
        public long AttrId { get; set; }
        /// <summary>
        /// 属性分组Id
        /// </summary>
        public long AttrGroupId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
    }
}
