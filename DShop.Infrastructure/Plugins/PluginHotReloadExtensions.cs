using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DShop.Infrastructure.Plugins;

/// <summary>
/// 插件热更新服务的 DI 扩展方法，供 API 项目一键集成
/// </summary>
public static class PluginHotReloadExtensions
{
    /// <summary>
    /// 注册插件热更新所需的所有服务：
    /// - PluginControllerActivator（自定义控制器激活器，让插件控制器走插件 DI）
    /// - PluginActionDescriptorChangeProvider（热更新时刷新路由）
    /// - PluginHotReloadService（核心后台服务，加载/监控/卸载插件）
    /// </summary>
    /// <param name="services">DI 容器</param>
    /// <param name="sharedTypes">
    /// 需要从宿主（API 主项目）解析的共享类型列表。
    /// 添加代表类型即可让 PluginLoader 从主项目解析整个程序集，
    /// 避免插件目录中缺少依赖 DLL 导致 ReflectionTypeLoadException。
    /// 通常放 EF Core DbContext 和 Model 实体类型。
    /// </param>
    public static IServiceCollection AddPluginHotReload(
        this IServiceCollection services,
        IEnumerable<Type> sharedTypes)
    {
        services.AddSingleton<IControllerActivator, PluginControllerActivator>();
        services.AddSingleton<IActionDescriptorChangeProvider>(_ => PluginActionDescriptorChangeProvider.Provider);
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var partManager = sp.GetRequiredService<ApplicationPartManager>();
            var logger = sp.GetRequiredService<ILogger<PluginHotReloadService>>();
            return new PluginHotReloadService(config, partManager, logger, sp, sharedTypes);
        });
        services.AddHostedService(sp => sp.GetRequiredService<PluginHotReloadService>());
        return services;
    }
}
