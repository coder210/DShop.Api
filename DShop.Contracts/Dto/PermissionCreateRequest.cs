namespace DShop.Contracts.Dto
{
    public class PermissionCreateRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
    }
}
