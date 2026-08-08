using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.PluginShared;
using Microsoft.Extensions.Configuration;

namespace DShop.AdminPlugin.Services
{
    public class SystemQueryService : ISystemQueryService
    {
        private readonly DatabaseContext _context;
        private readonly IUserContext _userContext;
        private readonly string _basePath;

        public SystemQueryService(DatabaseContext context, IUserContext userContext, IConfiguration configuration)
        {
            _context = context;
            _userContext = userContext;
            _basePath = Path.Combine(configuration[Constants.FileStorageBasePath] ?? "D:/Uploads/", "Templates");
        }

        public PagedResponse<AuditLogListResponse> GetAuditLogList(
            string? keyword,
            string? action,
            string? tableName,
            DateTime? dateFrom,
            DateTime? dateTo,
            int page,
            int size)
        {
            var query = _context.AuditLogs
                .GroupJoin(_context.Users,
                           log => log.OperatorId, u => u.Id,
                           (log, users) => new { log, users })
                .SelectMany(x => x.users.DefaultIfEmpty(),
                            (x, user) => new { x.log, OperatorName = user != null ? user.Username : "未知" });

            if (!string.IsNullOrWhiteSpace(action))
            {
                var actions = action.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim())
                    .ToList();
                query = query.Where(x => actions.Contains(x.log.Action));
            }

            if (!string.IsNullOrWhiteSpace(tableName))
            {
                query = query.Where(x => x.log.TableName.Contains(tableName));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.log.TableName.Contains(keyword) ||
                    x.OperatorName.Contains(keyword));
            }

            if (dateFrom.HasValue)
                query = query.Where(x => x.log.OperationTime >= dateFrom.Value);
            if (dateTo.HasValue)
                query = query.Where(x => x.log.OperationTime <= dateTo.Value.AddDays(1));

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(x => x.log.OperationTime)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(x => new AuditLogListResponse
                {
                    Id = x.log.Id,
                    TableName = x.log.TableName,
                    RecordId = x.log.RecordId,
                    Action = x.log.Action,
                    ActionDisplay = x.log.Action == "Added" ? "新增"
                        : x.log.Action == "Modified" ? "修改"
                        : x.log.Action == "Deleted" ? "删除" : x.log.Action,
                    OperatorId = x.log.OperatorId,
                    OperatorName = x.OperatorName,
                    OperationTime = x.log.OperationTime
                })
                .ToList();

            return new PagedResponse<AuditLogListResponse>
            {
                TotalCount = totalCount,
                PageIndex = page,
                PageSize = size,
                Items = items
            };
        }

        public AuditLogDetailResponse? GetAuditLogDetail(long id, out string msg)
        {
            var record = _context.AuditLogs
                .Where(log => log.Id == id)
                .GroupJoin(_context.Users,
                           log => log.OperatorId, u => u.Id,
                           (log, users) => new { log, users })
                .SelectMany(x => x.users.DefaultIfEmpty(),
                            (x, user) => new AuditLogDetailResponse
                            {
                                Id = x.log.Id,
                                TableName = x.log.TableName,
                                RecordId = x.log.RecordId,
                                Action = x.log.Action,
                                ActionDisplay = x.log.Action == "Added" ? "新增"
                                    : x.log.Action == "Modified" ? "修改"
                                    : x.log.Action == "Deleted" ? "删除" : x.log.Action,
                                OldValueJson = x.log.OldValueJson,
                                NewValueJson = x.log.NewValueJson,
                                OperatorId = x.log.OperatorId,
                                OperatorName = user != null ? user.Username : "未知",
                                OperationTime = x.log.OperationTime
                            })
                .FirstOrDefault();

            if (record == null)
            {
                msg = "审计日志不存在";
                return null;
            }

            msg = "获取成功";
            return record;
        }

        public PagedResponse<TemplateResponse> GetTemplateList(string? keyword, int pageIndex, int pageSize)
        {
            var query = _context.DocumentTemplates
                .Where(t => !t.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t =>
                    t.TemplateName.Contains(keyword) ||
                    t.TemplateCode.Contains(keyword) ||
                    t.DocumentType.Contains(keyword));
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TemplateResponse
                {
                    Id = t.Id,
                    Name = t.TemplateName,
                    Type = t.DocumentType,
                    Remark = t.Remark,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            return new PagedResponse<TemplateResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public TemplateResponse? GetTemplateById(long id)
        {
            var entity = _context.DocumentTemplates
                .FirstOrDefault(t => t.Id == id && !t.IsDeleted);

            if (entity == null) return null;

            return new TemplateResponse
            {
                Id = entity.Id,
                Name = entity.TemplateName,
                Type = entity.DocumentType,
                Content = ReadTemplateContent(entity.FilePath),
                Remark = entity.Remark,
                CreatedAt = entity.CreatedAt
            };
        }

        private static string ReadTemplateContent(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return string.Empty;

            try
            {
                return File.ReadAllText(filePath);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
