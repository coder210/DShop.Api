using Api.Filters;
using DShop.Infrastructure;
using DShop.Infrastructure.Plugins;
using DShop.PluginShared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString,
        sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(3);
            sqlServerOptions.CommandTimeout(30);
        });
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

// ==================== 中间件管道 ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
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
