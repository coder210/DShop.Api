using Api.Filters;
using DShop.Infrastructure;
using DShop.Infrastructure.Plugins;
using DShop.PluginShared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==================== Serilog 日志 ====================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "dshop-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();
builder.Host.UseSerilog();

// ==================== 服务注册 ====================

// --- Controllers + Filters ---
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<OperationLogFilter>();
    options.Filters.Add<PermissionAuthorizationFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddEndpointsApiExplorer();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- JWT 认证 ---
var jwtSection = builder.Configuration.GetSection("JWT");
var jwtKey = Encoding.UTF8.GetBytes(jwtSection["IssuerSigningKey"]!);
JwtHelper.Configure(jwtSection["IssuerSigningKey"]!, jwtSection["ValidIssuer"]!, jwtSection["ValidAudience"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["ValidIssuer"],
        ValidAudience = jwtSection["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true,
        // Token 在黑名单中则不通过验证
        LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
        {
            if (expires != null && expires < DateTime.UtcNow) return false;
            return true;
        }
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // 从JWT中提取用户ID(sub)，查询有效Token验证
            var userClaim = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                         ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(userClaim, out var userId))
            {
                try
                {
                    // 从DI容器解析服务，替代原来的new IdentityQueryService()
                    var identityQuery = context.HttpContext.RequestServices
                        .GetRequiredService<ITokenValidator>();
                    if (!identityQuery.ValidateToken(userId, out var tokenInfo))
                    {
                        context.Fail("Token不存在或已过期");
                        return Task.CompletedTask;
                    }
                    if (tokenInfo!.ExpiresAt != null && tokenInfo.ExpiresAt < DateTime.UtcNow)
                    {
                        context.Fail("Token已过期");
                        return Task.CompletedTask;
                    }
                }
                catch (Exception)
                {
                    context.Fail("身份验证服务不可用");
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

// --- 基础服务 ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<ITokenValidator, TokenValidator>();
builder.Services.AddScoped<IPermissionSeedService, PermissionSeedService>();

// --- EF Core DbContext ---
// 根据连接字符串自动选择数据库提供程序：
//   含 .db / .sqlite / :memory: 且非服务器地址 -> SQLite
//   其它（如 "Data Source=host,port;..."） -> SQL Server
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var isSqlite = !string.IsNullOrEmpty(connectionString)
        && (connectionString.Contains(".db", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains(".sqlite", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains(":memory:", StringComparison.OrdinalIgnoreCase));

    if (isSqlite)
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString,
            sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(3);
                sqlServerOptions.CommandTimeout(30);
            });
    }
}, ServiceLifetime.Scoped);

// --- Swagger ---
builder.Services.AddSwaggerGen();

// --- 插件热更新（真正的进程内热更新，无需重启）---
builder.Services.AddPluginHotReload(sharedTypes:
[
    typeof(DShop.Infrastructure.DatabaseContext),
    typeof(DShop.Models.User),
    typeof(DShop.Models.Menu),
    typeof(DShop.Models.Permission),
]);

var app = builder.Build();

// ==================== 数据库自动建表 + 种子数据 ====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    // 开发阶段使用 EnsureCreated 直接按模型建表（SQLite / SqlServer 均适用），
    // 避免迁移历史与模型不一致导致"no such table"。
    db.Database.EnsureCreated();

    var permissionSeed = scope.ServiceProvider.GetRequiredService<IPermissionSeedService>();
    DbSeeder.Seed(db, permissionSeed);
}

// ==================== 中间件管道 ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

// 图片访问：/images/xxx.png -> {FileStorage:BasePath}/images/xxx.png
var fileStorageBasePath = builder.Configuration[Constants.FileStorageBasePath] ?? "D:/Uploads/";
var imagesDir = Path.Combine(fileStorageBasePath, "images");
if (!Directory.Exists(imagesDir))
{
    Directory.CreateDirectory(imagesDir);
}
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/images",
    FileProvider = new PhysicalFileProvider(imagesDir)
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 使用Serilog请求日志
app.UseSerilogRequestLogging();

// ==================== 启动初始化 ====================
Log.Information("DShop API 启动完成，热更新服务就绪后自动加载插件");
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "应用程序启动失败，发生致命错误");
    throw;
}
