using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Infrastructure.Plugins;

/// <summary>
/// 自定义控制器激活器，使插件控制器从插件自己的 DI 容器中解析依赖
/// 并跟踪活跃请求数，确保旧 ALC 在所有请求完成后再卸载
/// </summary>
public class PluginControllerActivator : IControllerActivator
{
    public object Create(ControllerContext context)
    {
        var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();

        // 判断是否为插件控制器（从 PluginHotReloadService 的上下文中查找）
        var pluginService = context.HttpContext.RequestServices
            .GetRequiredService<PluginHotReloadService>();

        var instance = pluginService.ResolveController(controllerType, context.HttpContext.RequestServices);
        if (instance != null)
        {
            // 插件控制器：增加活跃请求计数，待 Release 时递减
            pluginService.TrackRequestStart(controllerType.Assembly);
            return instance;
        }

        // 非插件控制器，直接用 DI 容器解析
        return ActivatorUtilities.CreateInstance(
            context.HttpContext.RequestServices, controllerType);
    }

    public void Release(ControllerContext context, object controller)
    {
        var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
        var pluginService = context.HttpContext.RequestServices
            .GetRequiredService<PluginHotReloadService>();

        // 通知插件服务该请求已结束（若来自插件则递减计数，非插件则为安全空操作）
        pluginService.TrackRequestEnd(controllerType.Assembly);

        if (controller is IDisposable disposable)
            disposable.Dispose();
    }
}
