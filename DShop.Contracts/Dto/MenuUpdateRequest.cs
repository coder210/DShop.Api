namespace DShop.Contracts.Dto
{
    public class MenuUpdateRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Icon { get; set; }

        public long ParentId { get; set; }

        public int SortOrder { get; set; }
    }
}
