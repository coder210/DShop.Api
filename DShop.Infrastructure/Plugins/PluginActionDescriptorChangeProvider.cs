using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace DShop.Infrastructure.Plugins;

/// <summary>
/// 用于在运行时通知 ASP.NET Core MVC 刷新 ActionDescriptor（路由/控制器缓存）
/// 当插件热更新时，通过此 Provider 触发 MVC 重新发现控制器
/// </summary>
public class PluginActionDescriptorChangeProvider : IActionDescriptorChangeProvider
{
    private static readonly ActionDescriptorChangeProviderInstance Instance = new();

    public static IActionDescriptorChangeProvider Provider => Instance;

    /// <summary>
    /// 调用此方法通知 MVC 重新计算所有 Action 描述符
    /// </summary>
    public static void NotifyChange()
    {
        Instance.Notify();
    }

    public IChangeToken GetChangeToken()
    {
        return Instance.GetChangeToken();
    }

    /// <summary>
    /// 内部实现，使用 CancellationChangeToken 机制
    /// </summary>
    private class ActionDescriptorChangeProviderInstance : IActionDescriptorChangeProvider
    {
        private CancellationTokenSource _cts = new();

        public void Notify()
        {
            var old = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
            old.Cancel();
            old.Dispose();
        }

        public IChangeToken GetChangeToken()
        {
            return new CancellationChangeToken(_cts.Token);
        }
    }
}
