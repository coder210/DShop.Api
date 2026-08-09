namespace DShop.Contracts.Dto
{
    /// <summary>
    /// 用户可见菜单节点（含来源标注）。
    /// source 取值：direct=直接绑定(UserMenus) / role=来自角色(RoleMenus) / both=两者都有
    /// </summary>
    public class VisibleMenuResponse
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
        /// <summary>
        /// 菜单来源：direct / role / both
        /// </summary>
        public string Source { get; set; }
        public List<VisibleMenuResponse> Children { get; set; } = new List<VisibleMenuResponse>();
    }
}
