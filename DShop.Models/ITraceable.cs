namespace DShop.Models
{
    /// <summary>
    /// 溯源核心接口（所有实体必须实现）
    /// </summary>
    internal interface ITraceable
    {
        long Id { get; set; }
        DateTime CreatedAt { get; set; }
        long CreatedBy { get; set; }
        DateTime ModifiedAt { get; set; }
        long ModifiedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
