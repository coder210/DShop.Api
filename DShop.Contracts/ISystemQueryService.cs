using DShop.Contracts.Dto;

namespace DShop.Contracts
{
    public interface ISystemQueryService
    {
        /// <summary>分页查询审计日志</summary>
        PagedResponse<AuditLogListResponse> GetAuditLogList(
            string? keyword,
            string? action,
            string? tableName,
            DateTime? dateFrom,
            DateTime? dateTo,
            int page,
            int size);

        /// <summary>根据ID获取审计日志详情</summary>
        AuditLogDetailResponse? GetAuditLogDetail(long id, out string msg);

        /// <summary>获取模板列表（分页）</summary>
        PagedResponse<TemplateResponse> GetTemplateList(string? keyword, int pageIndex, int pageSize);

        /// <summary>根据ID获取模板</summary>
        TemplateResponse? GetTemplateById(long id);
    }
}
