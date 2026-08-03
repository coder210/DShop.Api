namespace DShop.Contracts.Dto
{
    public class MenuResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }

        public List<MenuResponse> Children { get; set; } = new List<MenuResponse>();
    }
}
