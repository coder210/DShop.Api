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

        /// <summary>
        /// 绑定的后端控制器名称（去 Controller 后缀），仅叶子功能菜单填写。
        /// </summary>
        public string Controller { get; set; }

        [Required]
        public long ParentId { get; set; }

        [Required]
        public int SortOrder { get; set; }
    }
}
