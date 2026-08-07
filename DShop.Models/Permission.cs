using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 权限表
    /// </summary>
    [Table("Permissions")]
    public class Permission
    {
        [Key]
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 权限标识：如 'quote:fc:view'
        /// </summary>
        public string PermissionCode { get; set; }
        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 归属模块（权限码去除端前缀后的第一段，kebab-case），如 user、role-management。
        /// 用于在前端按菜单/模块分组展示权限，解决"某菜单一半权限难找"的问题。
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// 权限所属端（客户端）：admin / app。
        /// 与 PermissionCode 中的 "admin::" 前缀含义一致，但独立成列便于按端查询/统计/索引；
        /// PermissionCode 仍保留完整前缀形式（如 admin::user:list），以兼容 JWT 与鉴权链路。
        /// 默认 admin；未来独立的 app 端控制器在 [AuthorizePermission(Client = "app")] 中显式声明。
        /// </summary>
        public string Client { get; set; }
        /// <summary>
        /// 接口是否仍存在：代码中存在为 true(1)，代码中已删除（无对应 [AuthorizePermission]）为 false(0)。
        /// 不物理删除记录，以保留角色绑定历史；种子同步时根据扫描结果刷新此标记。
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 真实接口端点：控制器类名 + Action 方法名，如 EntrustLedgerController.GetList。
        /// 与路由无关，仅随类名/方法名变化，用于后端排查代码时直接定位接口实现。
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// 真实 HTTP 路由：HTTP 方法 + 路由模板，如 GET /api/admin/EntrustLedger/GetList。
        /// 与浏览器 Network 中看到的 URL 一致，便于前端对照接口；带参数的路由保留占位符（如 {id}）。
        /// </summary>
        public string ApiPath { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
