namespace DShop.Contracts.Dto
{
    public class PermissionUpdateRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
    }
}
