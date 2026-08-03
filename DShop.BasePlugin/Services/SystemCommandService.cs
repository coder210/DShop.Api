using DShop.Contracts;
using DShop.Contracts.Dto;
using DShop.Infrastructure;
using DShop.PluginShared;
using DShop.Models;
using Microsoft.Extensions.Configuration;

namespace DShop.BasePlugin.Services
{
    public class SystemCommandService : ISystemCommandService
    {
        private readonly DatabaseContext _context;
        private readonly IUserContext _userContext;
        private readonly string _basePath;

        public SystemCommandService(DatabaseContext context, IUserContext userContext, IConfiguration configuration)
        {
            _context = context;
            _userContext = userContext;
            _basePath = Path.Combine(configuration[Constants.FileStorageBasePath] ?? "D:/Uploads/", "Templates");
        }

        public bool CreateTemplate(CreateTemplateRequest request, out string msg)
        {
            if (_context.DocumentTemplates.Any(t => t.TemplateName == request.Name && !t.IsDeleted))
            {
                msg = "模板名称已存在";
                return false;
            }

            var codePrefix = "TMP_";
            var lastCode = _context.DocumentTemplates
                .Where(t => t.TemplateCode.StartsWith(codePrefix))
                .OrderByDescending(t => t.TemplateCode)
                .Select(t => t.TemplateCode)
                .FirstOrDefault();

            int nextSeq = 1;
            if (lastCode != null && int.TryParse(lastCode[4..], out int lastSeq))
                nextSeq = lastSeq + 1;

            var templateCode = $"{codePrefix}{nextSeq:D5}";

            var filePath = SaveTemplateContent(templateCode, request.Content);

            var entity = new DocumentTemplate
            {
                TemplateCode = templateCode,
                TemplateName = request.Name,
                DocumentType = request.Type,
                SubType = string.Empty,
                FilePath = filePath,
                FileVersion = "1.0",
                IsActive = true,
                Remark = request.Remark ?? string.Empty,
                IsDeleted = false,
                CreatedBy = _userContext.CurrentUserId,
                ModifiedBy = _userContext.CurrentUserId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            _context.DocumentTemplates.Add(entity);
            var saved = _context.SaveChanges() > 0;

            if (!saved)
            {
                DeleteTemplateFile(filePath);
            }

            msg = saved ? "创建成功" : "创建失败";
            return saved;
        }

        public bool UpdateTemplate(UpdateTemplateRequest request, out string msg)
        {
            var entity = _context.DocumentTemplates
                .FirstOrDefault(t => t.Id == request.Id && !t.IsDeleted);

            if (entity == null)
            {
                msg = "模板不存在";
                return false;
            }

            if (_context.DocumentTemplates.Any(t =>
                    t.TemplateName == request.Name &&
                    t.Id != request.Id &&
                    !t.IsDeleted))
            {
                msg = "模板名称已存在";
                return false;
            }

            var filePath = SaveTemplateContent(entity.TemplateCode, request.Content);
            var oldFilePath = entity.FilePath;

            entity.TemplateName = request.Name;
            entity.DocumentType = request.Type;
            entity.FilePath = filePath;
            entity.Remark = request.Remark ?? string.Empty;
            entity.ModifiedBy = _userContext.CurrentUserId;
            entity.ModifiedAt = DateTime.UtcNow;

            var saved = _context.SaveChanges() > 0;

            if (saved)
            {
                if (!string.IsNullOrEmpty(oldFilePath) && oldFilePath != filePath)
                {
                    DeleteTemplateFile(oldFilePath);
                }
            }
            else
            {
                DeleteTemplateFile(filePath);
            }

            msg = saved ? "更新成功" : "更新失败";
            return saved;
        }

        public bool DeleteTemplate(long id, out string msg)
        {
            var entity = _context.DocumentTemplates
                .FirstOrDefault(t => t.Id == id && !t.IsDeleted);

            if (entity == null)
            {
                msg = "模板不存在";
                return false;
            }

            var filePath = entity.FilePath;

            entity.IsDeleted = true;
            entity.ModifiedBy = _userContext.CurrentUserId;
            entity.ModifiedAt = DateTime.UtcNow;

            var saved = _context.SaveChanges() > 0;

            if (saved)
            {
                DeleteTemplateFile(filePath);
            }

            msg = saved ? "删除成功" : "删除失败";
            return saved;
        }

        private string SaveTemplateContent(string templateCode, string? content)
        {
            var fileName = $"{templateCode}.html";
            var fullPath = Path.Combine(_basePath, fileName);

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            File.WriteAllText(fullPath, content ?? string.Empty);

            return fullPath;
        }

        private static void DeleteTemplateFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            try
            {
                File.Delete(filePath);
            }
            catch
            {
            }
        }
    }
}
