namespace DShop.Contracts.Dto
{
    public class CreateTemplateRequest
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 模板类型
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
