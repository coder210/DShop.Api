using DShop.AdminPlugin.Services;
using DShop.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.AdminPlugin;

public static class AdminPluginService
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Identity services
        services.AddScoped<IIdentityQueryService, IdentityQueryService>();
        services.AddScoped<IIdentityCommandService, IdentityCommandService>();

        // System services
        services.AddScoped<ISystemQueryService, SystemQueryService>();
        services.AddScoped<ISystemCommandService, SystemCommandService>();

        // 电商领域服务（Product / Customer / Order）
        services.AddScoped<IProductQueryService, ProductQueryService>();
        services.AddScoped<IProductCommandService, ProductCommandService>();
        services.AddScoped<ICustomerQueryService, CustomerQueryService>();
        services.AddScoped<ICustomerCommandService, CustomerCommandService>();
        services.AddScoped<IOrderQueryService, OrderQueryService>();
        services.AddScoped<IOrderCommandService, OrderCommandService>();

        // 首页看板
        services.AddScoped<IDashboardQueryService, DashboardQueryService>();

        // 售后/退款
        services.AddScoped<IRefundQueryService, RefundQueryService>();
        services.AddScoped<IRefundCommandService, RefundCommandService>();
    }
}
