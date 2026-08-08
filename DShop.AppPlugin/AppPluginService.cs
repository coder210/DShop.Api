using DShop.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.AppPlugin;

/// <summary>
/// App 端(前端 App)插件入口。
/// 框架按 "*PluginService" 后缀 + ConfigureServices 方法名自动发现并调用。
/// 所有控制器统一使用 api/app 路由前缀。
/// </summary>
public static class AppPluginService
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // TODO: 在此注册 App 端服务（顾客登录、商品、订单等）
        // 示例：services.AddScoped<ICustomerService, CustomerService>();
    }
}
