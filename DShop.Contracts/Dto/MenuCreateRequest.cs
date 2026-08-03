using System.ComponentModel.DataAnnotations;

namespace DShop.Contracts.Dto
{
    public class MenuCreateRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Path { get; set; }

        public string Icon { get; set; }

        [Required]
        public long ParentId { get; set; }

        [Required]
        public int SortOrder { get; set; }
    }
}
