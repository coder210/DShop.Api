namespace DShop.Contracts.Dto
{
    public class MenuResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        /// <summary>
        /// 绑定的后端控制器名称（去 Controller 后缀），目录型菜单为空。
        /// </summary>
        public string Controller { get; set; }

        public List<MenuResponse> Children { get; set; } = new List<MenuResponse>();
    }
}
