using DShop.Models;
using DShop.PluginShared;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DShop.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        private readonly IUserContext _userContext;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IUserContext userContext)
            : base(options)
        {
            _userContext = userContext;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 将来可以在这里配置 Fluent API
        }

        // ========== 同步版本 ==========
        public override int SaveChanges()
        {
            // 禁止手动修改或删除 AuditLog（系统自动插入是允许的）
            var forbidden = ChangeTracker.Entries<AuditLog>()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList();
            if (forbidden.Any())
                throw new InvalidOperationException("审计日志表禁止手动修改或删除。");

            // 生成审计日志
            var auditLogs = GenerateAuditLogs();

            // 同步插入审计日志
            if (auditLogs.Any())
            {
                Set<AuditLog>().AddRange(auditLogs);
            }

            return base.SaveChanges();
        }

        // ========== 异步无参版本（转发到带参重载） ==========
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(true, cancellationToken);
        }

        // ========== 异步带参版本 ==========
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            // 禁止手动修改或删除 AuditLog
            var forbidden = ChangeTracker.Entries<AuditLog>()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList();
            if (forbidden.Any())
                throw new InvalidOperationException("审计日志表禁止手动修改或删除。");

            // 生成审计日志（同步逻辑，无异步 I/O）
            var auditLogs = GenerateAuditLogs();

            // 异步插入审计日志
            if (auditLogs.Any())
            {
                await Set<AuditLog>().AddRangeAsync(auditLogs, cancellationToken);
            }

            // ★ 关键修正：传入 acceptAllChangesOnSuccess 参数给基类 ★
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        // ========== 私有方法：生成审计日志（同步，无异步依赖） ==========
        private List<AuditLog> GenerateAuditLogs()
        {
            // 抓取所有变更的业务实体（排除 AuditLog 自身）
            var entries = ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added ||
                             e.State == EntityState.Modified ||
                             e.State == EntityState.Deleted) &&
                            !(e.Entity is AuditLog))
                .ToList();

            var auditLogs = new List<AuditLog>();
            var currentUserId = _userContext.CurrentUserId; // 未认证时记录为系统操作

            foreach (var entry in entries)
            {
                var log = new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    RecordId = entry.State == EntityState.Added ? 0 : (long)entry.Property("Id").CurrentValue,
                    Action = entry.State.ToString(),
                    OperatorId = currentUserId,
                    NewValueJson = string.Empty,
                    OldValueJson = string.Empty,
                    OperationTime = DateTime.UtcNow
                };

                // 根据操作类型记录前后值
                if (entry.State == EntityState.Modified)
                {
                    var oldValues = new Dictionary<string, object>();
                    var newValues = new Dictionary<string, object>();

                    // 只记录标量属性（忽略导航属性），且只记录真正发生了变化的字段
                    var scalarProperties = entry.Properties
                        .Where(p => !p.Metadata.IsPrimaryKey() && !p.Metadata.IsForeignKey());

                    foreach (var property in scalarProperties)
                    {
                        if (property.IsModified)
                        {
                            oldValues[property.Metadata.Name] = property.OriginalValue ?? "NULL";
                            newValues[property.Metadata.Name] = property.CurrentValue ?? "NULL";
                        }
                    }

                    log.OldValueJson = JsonSerializer.Serialize(oldValues);
                    log.NewValueJson = JsonSerializer.Serialize(newValues);
                }
                else if (entry.State == EntityState.Added)
                {
                    var newValues = entry.Properties
                        .Where(p => !p.Metadata.IsPrimaryKey() && !p.Metadata.IsForeignKey())
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue ?? "NULL");

                    log.NewValueJson = JsonSerializer.Serialize(newValues);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    var oldValues = entry.Properties
                        .Where(p => !p.Metadata.IsPrimaryKey() && !p.Metadata.IsForeignKey())
                        .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue ?? "NULL");

                    log.OldValueJson = JsonSerializer.Serialize(oldValues);
                }

                auditLogs.Add(log);
            }

            return auditLogs;
        }

        // ========== DbSet 定义 ==========
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserMenu> UserMenus { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        // 角色模块（RBAC）
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }

        /// <summary>
        /// 模版表
        /// </summary>
        public DbSet<DocumentTemplate> DocumentTemplates { get; set; }

    }
}