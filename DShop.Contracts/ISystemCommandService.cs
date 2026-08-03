using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    public interface ISystemCommandService
    {
        /// <summary>创建模板</summary>
        bool CreateTemplate(CreateTemplateRequest request, out string msg);

        /// <summary>更新模板</summary>
        bool UpdateTemplate(UpdateTemplateRequest request, out string msg);

        /// <summary>删除模板</summary>
        bool DeleteTemplate(long id, out string msg);
    }
}
