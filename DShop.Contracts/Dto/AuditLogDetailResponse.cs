namespace DShop.Contracts.Dto
{
    public class AuditLogDetailResponse
    {
        public long Id { get; set; }
        public string TableName { get; set; } = null!;
        public long RecordId { get; set; }
        public string Action { get; set; } = null!;
        public string? ActionDisplay { get; set; }
        public string? OldValueJson { get; set; }
        public string? NewValueJson { get; set; }
        public long OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public DateTime OperationTime { get; set; }
    }
}
