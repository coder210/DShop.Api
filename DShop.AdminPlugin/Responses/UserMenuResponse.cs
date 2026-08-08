namespace DShop.AdminPlugin.Responses
{
    public class UserMenuResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Path { get; set; }
        public int SortOrder { get; set; }
        public List<UserMenuResponse> Children { get; set; }
    }
}
