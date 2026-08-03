using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DShop.Models
{
    /// <summary>
    /// 系统审计日志(只能查询)
    /// </summary>
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(50)] 
        public string TableName { get; set; }
        public long RecordId { get; set; }
        [MaxLength(20)] 
        /// <summary>
        /// Insert/Update/Delete
        /// </summary>
        public string Action { get; set; }
        public string OldValueJson { get; set; }
        public string NewValueJson { get; set; }
        public long OperatorId { get; set; }
        public DateTime OperationTime { get; set; }
    }
}
