using DShop.Models;

namespace DShop.Contracts.Dto
{
    public class PermissionResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<Menu>? RelatedMenus { get; set; }
    }
}
