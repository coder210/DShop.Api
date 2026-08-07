using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 菜单表
    /// </summary>
    [Table("Menus")]
    public class Menu
    {
        [Key]
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public long ParentId { get; set; }
        /// <summary>
        /// 绑定的后端控制器名称（去 Controller 后缀），如 EntrustLedger。
        /// 仅叶子功能菜单填写，表示该菜单页使用此控制器的全部 Action，且只使用此控制器的接口。
        /// 与权限表的 Module 字段使用同一套取值，建立菜单与权限的业务关联契约。
        /// 目录型父菜单不绑定控制器，留空。
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
