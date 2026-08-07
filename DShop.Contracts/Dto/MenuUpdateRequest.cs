namespace DShop.Contracts.Dto
{
    public class MenuUpdateRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Icon { get; set; }

        /// <summary>
        /// 绑定的后端控制器名称（去 Controller 后缀），仅叶子功能菜单填写。
        /// </summary>
        public string Controller { get; set; }

        public long ParentId { get; set; }

        public int SortOrder { get; set; }
    }
}
