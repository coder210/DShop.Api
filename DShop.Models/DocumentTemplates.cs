using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{

    [Table("DocumentTemplates")]
    public class DocumentTemplate : ITraceable
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(50)] 
        /// <summary>
        /// 如 "TMP_ORDER_001"
        /// </summary>
        public string TemplateCode { get; set; }
        [MaxLength(100)] 
        /// <summary>
        /// 如 "用户信息导出表"
        /// </summary>
        public string TemplateName { get; set; }
        [MaxLength(50)] 
        /// <summary>
        /// 文档类型：用于区分模板适用的业务单据（英文枚举，如 Order、Report 等）。
        /// </summary>
        public string DocumentType { get; set; }
        [MaxLength(50)]
        /// <summary>
        /// 子类型：废气/空气/水质（如果通用版式不同）
        /// </summary>
        public string SubType { get; set; }
        [MaxLength(500)] 
        /// <summary>
        /// 物理路径（Word/Excel/HTML模板）
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 模板内容（HTML 片段），用于在线渲染打印
        /// </summary>
        public string? TemplateContent { get; set; }
        [MaxLength(20)] 
        /// <summary>
        /// 版本号
        /// </summary>
        public string FileVersion { get; set; } = "1.0";
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; } = true;
        [MaxLength(500)] 
        /// <summary>
        /// 备注（如“CMA章位置调整”）
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        [Required]
        /// <summary>
        /// 最后一个修改人的id
        /// </summary>
        public long ModifiedBy { get; set; }
        [Required]
        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        public DateTime ModifiedAt { get; set; }
        [Required]
        /// <summary>
        /// 创建人的id
        /// </summary>
        public long CreatedBy { get; set; }
        [Required]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

    }
}
