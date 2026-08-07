using McMaster.NETCore.Plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace DShop.Infrastructure.Plugins;

/// <summary>
/// 热更新核心服务 — 真正的进程内热更新，无需重启应用
///
/// 工作原理：
/// 1. 每个插件使用独立的 AssemblyLoadContext（可卸载）
/// 2. 插件服务注册到插件自己的 IServiceProvider，不污染主 DI
/// 3. 通过 ApplicationPartManager 动态添加/移除控制器
/// 4. 通过 ActionDescriptorChangeProvider 通知 MVC 刷新路由
/// 5. 影子复制避免 DLL 文件锁定，同时保证 ALC 能加载到全部依赖程序集
///
/// 不依赖 IIS，Kestrel / nginx / 纯进程均可使用
/// </summary>
public class PluginHotReloadService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationPartManager _partManager;
    private readonly ILogger<PluginHotReloadService> _logger;
    // 主 DI 容器的 ServiceProvider，构造函数注入
    private readonly IServiceProvider _mainServiceProvider;

    // 所有已加载的插件上下文（按插件名称索引）
    private readonly ConcurrentDictionary<string, PluginContext> _plugins = new();

    // 待卸载的旧插件上下文（热更新后，等待活跃请求清零再真正卸载）
    // 用 PluginContext 引用作 key，允许多个版本共存，避免多轮更新覆盖泄漏
    private readonly ConcurrentDictionary<PluginContext, byte> _pendingUnload = new();

    // 文件监听
    private FileSystemWatcher? _watcher;
    private Timer? _debounceTimer;
    private readonly ConcurrentBag<string> _pendingChanges = new();
    private readonly object _lock = new();

    private readonly Type[] _sharedTypes;

    // 配置
    private string PluginDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
    private string ShadowDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PluginsCache");
    private int DebounceMs => _configuration.GetValue("PluginHotReload:DebounceMilliseconds", 2000);
    private bool HotReloadEnabled => _configuration.GetValue("PluginHotReload:Enabled", true);

    public PluginHotReloadService(
        IConfiguration configuration,
        ApplicationPartManager partManager,
        ILogger<PluginHotReloadService> logger,
        IServiceProvider mainServiceProvider,
        IEnumerable<Type> sharedTypes)
    {
        _configuration = configuration;
        _partManager = partManager;
        _logger = logger;
        _mainServiceProvider = mainServiceProvider;
        _sharedTypes = sharedTypes.ToArray();
    }

    /// <summary>
    /// 被 PluginControllerActivator 调用：从请求作用域 + 插件容器联合解析控制器
    /// 遍历所有已加载及待卸载的插件，找到控制器所属的那个插件容器
    /// </summary>
    public object? ResolveController(Type controllerType, IServiceProvider requestServices)
    {
        // 先查已加载的插件（新请求走新版本）
        var ctx = FindPluginByAssembly(controllerType.Assembly, includePending: false);
        // 过渡期内若旧 ActionDescriptor 还在缓存，也要能从待卸载中找到旧版本
        ctx ??= FindPluginByAssembly(controllerType.Assembly, includePending: true);

        if (ctx != null)
        {
            return ActivatorUtilities.CreateInstance(
                new PluginAwareServiceProvider(requestServices, ctx.ServiceProvider, ctx.ServiceRegistrations),
                controllerType);
        }
        return null;
    }

    /// <summary>
    /// 根据控制器程序集查找所属插件上下文（跨 _plugins 和 _pendingUnload）
    /// </summary>
    private PluginContext? FindPluginByAssembly(Assembly assembly, bool includePending)
    {
        foreach (var kvp in _plugins)
            if (kvp.Value.Assembly == assembly) return kvp.Value;

        if (includePending)
        {
            foreach (var kvp in _pendingUnload)
                if (kvp.Key.Assembly == assembly) return kvp.Key;
        }
        return null;
    }

    /// <summary>
    /// 【请求入口】通知插件服务该请求正在使用某插件控制器
    /// 由 PluginControllerActivator.Create 调用
    /// </summary>
    public void TrackRequestStart(Assembly controllerAssembly)
    {
        var ctx = FindPluginByAssembly(controllerAssembly, includePending: true);
        if (ctx != null)
            Interlocked.Increment(ref ctx.ActiveRequestCount);
    }

    /// <summary>
    /// 【请求出口】通知插件服务该请求已结束使用某插件控制器
    /// 由 PluginControllerActivator.Release 调用
    /// </summary>
    public void TrackRequestEnd(Assembly controllerAssembly)
    {
        var ctx = FindPluginByAssembly(controllerAssembly, includePending: true);
        if (ctx != null)
            Interlocked.Decrement(ref ctx.ActiveRequestCount);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!HotReloadEnabled)
        {
            _logger.LogInformation("[HotReload] 插件热更新已禁用");
            return;
        }

        // 确保目录存在
        Directory.CreateDirectory(PluginDir);
        Directory.CreateDirectory(ShadowDir);

        // 首次启动：加载现有插件
        await LoadPluginsAsync(stoppingToken);

        // 启动文件监听
        StartFileWatcher();

        _logger.LogInformation("[HotReload] 热更新服务已就绪，监听目录: {Dir}", PluginDir);

        // 等待应用关闭
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) { }
        finally
        {
            Cleanup();
        }
    }

    // ======================== 插件加载 ========================

    private async Task LoadPluginsAsync(CancellationToken ct)
    {
        var pluginDlls = Directory.GetFiles(PluginDir, "DShop.*Plugin.dll");
        foreach (var dll in pluginDlls)
        {
            if (ct.IsCancellationRequested) break;
            await HotReloadPluginAsync(dll, isFirstLoad: true);
        }
    }

    /// <summary>
    /// 热更新（或首次加载）一个插件
    /// </summary>
    private async Task HotReloadPluginAsync(string dllPath, bool isFirstLoad = false)
    {
        try
        {
            // 1. 影子复制：将 DLL 复制到缓存目录，避免文件锁
            var shadowPath = ShadowCopy(dllPath);

            // 2. 加载新插件到隔离的 ALC
            var newContext = await Task.Run(() => LoadPluginInContext(shadowPath));
            if (newContext == null)
            {
                _logger.LogError("[HotReload] 插件加载失败: {Dll}", Path.GetFileName(dllPath));
                return;
            }

            var pluginName = Path.GetFileNameWithoutExtension(dllPath);
            var oldPlugin = _plugins.TryGetValue(pluginName, out var existing) ? existing : null;

            // 旧插件移入待卸载队列（继续为正在执行的旧请求服务，等待活跃请求归零）
            // 以 PluginContext 引用为 key，允许多个历史版本共存
            if (oldPlugin != null)
                _pendingUnload.TryAdd(oldPlugin, 0);

            _plugins[pluginName] = newContext;

            // 3. 更新 MVC ApplicationPartManager
            UpdateApplicationParts(oldPlugin, newContext);

            // 4. 通知 MVC 刷新路由
            PluginActionDescriptorChangeProvider.NotifyChange();

            // 5. 重新种子权限
            ReseedPermissions(newContext);

            if (isFirstLoad)
            {
                _logger.LogInformation("[HotReload] 插件首次加载完成: {Name} v{Version}",
                    newContext.Assembly.GetName().Name,
                    GetFileVersion(dllPath));
            }
            else
            {
                _logger.LogWarning("[HotReload] 插件热更新完成 → {Name} v{Version}",
                    newContext.Assembly.GetName().Name,
                    GetFileVersion(dllPath));
            }

            // 6. 异步等待旧插件请求处理完毕后再卸载
            if (oldPlugin != null)
            {
                _ = UnloadPluginAsync(oldPlugin);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[HotReload] 热更新失败: {Dll}", Path.GetFileName(dllPath));
        }
    }

    // ======================== 影子复制 ========================

    private string ShadowCopy(string sourceDll)
    {
        var version = DateTime.Now.ToString("yyyyMMddHHmmss");
        var cacheDir = Path.Combine(ShadowDir, version);
        Directory.CreateDirectory(cacheDir);

        // 复制插件目录下所有 DLL 和 PDB，确保 ALC 能找到所有依赖
        var pluginDir = Path.GetDirectoryName(sourceDll)!;
        foreach (var file in Directory.GetFiles(pluginDir, "*.dll"))
        {
            File.Copy(file, Path.Combine(cacheDir, Path.GetFileName(file)), overwrite: true);
        }
        foreach (var file in Directory.GetFiles(pluginDir, "*.pdb"))
        {
            File.Copy(file, Path.Combine(cacheDir, Path.GetFileName(file)), overwrite: true);
        }

        var dest = Path.Combine(cacheDir, Path.GetFileName(sourceDll));
        _logger.LogDebug("[HotReload] 影子复制: {Dir} → {Dest} ({Count} files)", 
            Path.GetFileName(pluginDir), cacheDir, 
            Directory.GetFiles(pluginDir, "*.dll").Length);
        return dest;
    }

    // ======================== ALC 隔离加载 ========================

    private PluginContext? LoadPluginInContext(string assemblyPath)
    {
        var loader = PluginLoader.CreateFromAssemblyFile(
            assemblyPath,
            sharedTypes: _sharedTypes,
            configure: config =>
            {
                config.IsUnloadable = true;       // 启用 ALC 卸载
                config.LoadInMemory = true;       // 加载到内存，不锁定文件
                config.PreferSharedTypes = true;  // 优先使用共享类型
            });

        var assembly = loader.LoadDefaultAssembly();

        // 使用 McMaster.NETCore.Plugins.Mvc 内部的 ApplicationPartFactory
        // 发现控制器、视图、Razor Pages 等所有 MVC 部件
        var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
        var applicationParts = partFactory.GetApplicationParts(assembly).ToArray();

        // 查找入口类（后缀 PluginService 且有 ConfigureServices 静态方法）
        var entryType = assembly.GetTypes()
            .FirstOrDefault(t => t.Name.EndsWith("PluginService")
                              && t.GetMethod("ConfigureServices",
                                     BindingFlags.Static | BindingFlags.Public) != null);

        // 构建插件独立的 ServiceProvider（返回内部容器 + 包装容器 + 注册信息）
        var (innerProvider, wrappedProvider, registrations) =
            BuildPluginServiceProvider(entryType, assembly);

        _logger.LogInformation("[HotReload] 已加载插件: {Assembly}, 入口: {Entry}, 部件数: {PartCount}",
            assembly.GetName().Name,
            entryType?.FullName ?? "无",
            applicationParts.Length);

        return new PluginContext
        {
            Loader = loader,
            Assembly = assembly,
            ApplicationParts = applicationParts,
            ServiceProvider = wrappedProvider,
            InnerProvider = innerProvider,
            ServiceRegistrations = registrations,
            EntryType = entryType,
        };
    }

    /// <summary>
    /// 为插件构建独立的 ServiceProvider，不污染主 DI 容器
    /// 返回 (内部纯容器, 包装容器, 服务注册信息) — 内部容器用于控制器解析（配合请求作用域），
    /// 包装容器供其他场景使用（回退到主根容器），注册信息用于 PluginAwareServiceProvider 递归构造
    /// </summary>
    private (IServiceProvider inner, IServiceProvider wrapped, IReadOnlyList<ServiceDescriptor> registrations)
        BuildPluginServiceProvider(Type? entryType, Assembly assembly)
    {
        var services = new ServiceCollection();

        // 调用插件的 ConfigureServices，注册插件特有的服务
        if (entryType != null)
        {
            var method = entryType.GetMethod("ConfigureServices",
                BindingFlags.Static | BindingFlags.Public);
            method?.Invoke(null, new object[] { services, _configuration });
        }

        // 自动注册插件程序集中的所有控制器，方便直接从容器解析
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
            {
                services.AddTransient(type);
            }
        }

        // 保存注册信息快照（供 PluginAwareServiceProvider 查找实现类型）
        var registrations = services.ToList().AsReadOnly();

        // 构建纯内部容器（不含主容器服务，仅插件自身注册的服务 + 控制器）
        var innerProvider = services.BuildServiceProvider();

        // 包装容器：回退到主根容器（用于非请求场景）
        var wrappedProvider = new PluginScopedServiceProvider(innerProvider, () => _mainServiceProvider);

        return (innerProvider, wrappedProvider, registrations);
    }

    // ======================== ApplicationPart 管理 ========================
    // 使用 McMaster.NETCore.Plugins.Mvc 内部相同的 ApplicationPartFactory 机制

    private void UpdateApplicationParts(PluginContext? oldPlugin, PluginContext newPlugin)
    {
        var parts = _partManager.ApplicationParts;

        // 移除旧的插件部件集合
        if (oldPlugin?.ApplicationParts != null)
        {
            foreach (var oldPart in oldPlugin.ApplicationParts)
            {
                parts.Remove(oldPart);
                _logger.LogDebug("[HotReload] 已移除旧部件: {Part}",
                    oldPart.Name);
            }
        }

        // 添加新插件的所有 MVC 部件（控制器、视图等）
        foreach (var part in newPlugin.ApplicationParts)
        {
            parts.Add(part);
            _logger.LogDebug("[HotReload] 已注册新部件: {Part} ({Type})",
                part.Name, part.GetType().Name);
        }
    }

    // ======================== 权限重新种子 ========================

    private void ReseedPermissions(PluginContext plugin)
    {
        try
        {
            using var scope = _mainServiceProvider.CreateScope();
            var seedService = scope.ServiceProvider.GetRequiredService<IPermissionSeedService>();
            // 传入当前所有已加载插件的程序集，保证权限表与控制器扫描覆盖全部插件，
            // 避免仅传入触发热更新的单个插件导致 _scannedAssemblies 缓存遗漏其它插件，
            // 进而使菜单-控制器一致性校验把其它插件的控制器误判为"不存在的控制器"。
            var allPluginAssemblies = _plugins.Values
                .Select(p => p.Assembly)
                .Distinct()
                .ToList();
            seedService.SeedPermissions(allPluginAssemblies);
            _logger.LogDebug("[HotReload] 权限种子已更新（覆盖 {Count} 个插件）", allPluginAssemblies.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HotReload] 权限更新失败（可忽略）");
        }
    }

    // ======================== 旧插件卸载 ========================

    /// <summary>
    /// 等待旧插件的所有活跃请求完成，再卸载程序集
    /// </summary>
    private async Task UnloadPluginAsync(PluginContext plugin)
    {
        try
        {
            // 等活跃请求全部结束（轮询，非忙等）
            while (Interlocked.CompareExchange(ref plugin.ActiveRequestCount, 0, 0) > 0)
            {
                await Task.Delay(500);
            }

            // 从待卸载队列中移除
            _pendingUnload.TryRemove(plugin, out _);

            // 释放插件 DI 容器
            if (plugin.ServiceProvider is IDisposable disposable)
                disposable.Dispose();

            // 释放 PluginLoader（触发 ALC 卸载）
            plugin.Loader?.Dispose();

            // 强制 GC 回收，使 ALC 真正卸载
            for (int i = 0; i < 3; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            _logger.LogInformation("[HotReload] 旧插件程序集已卸载: {Assembly}（活跃请求已清零）",
                plugin.Assembly.GetName().Name);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HotReload] 旧插件卸载时发生异常（不影响新插件）");

            // 保证从待卸载队列中移除
            _pendingUnload.TryRemove(plugin, out _);
        }
    }

    // ======================== 文件监听 ========================

    private void StartFileWatcher()
    {
        _watcher = new FileSystemWatcher(PluginDir)
        {
            Filter = "DShop.*Plugin.dll",
            NotifyFilter = NotifyFilters.FileName
                         | NotifyFilters.LastWrite
                         | NotifyFilters.CreationTime
                         | NotifyFilters.Size,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true,
            InternalBufferSize = 64 * 1024,
        };

        _watcher.Created += OnPluginFileChanged;
        _watcher.Changed += OnPluginFileChanged;
        _watcher.Deleted += OnPluginFileChanged;
        _watcher.Renamed += OnPluginChanged;
        _watcher.Error += OnWatcherError;

        _logger.LogInformation("[HotReload] 文件监听已启动");
    }

    private void OnPluginFileChanged(object sender, FileSystemEventArgs e)
    {
        // Changed 事件会触发多次（文件写入过程中），用防抖合并
        var fileName = Path.GetFileName(e.FullPath);
        _pendingChanges.Add($"[{e.ChangeType}] {fileName}");
        ResetDebounceTimer();
    }

    private void OnPluginChanged(object sender, RenamedEventArgs e)
    {
        _pendingChanges.Add($"[Renamed] {e.OldName} -> {e.Name}");
        ResetDebounceTimer();
    }

    private void OnWatcherError(object sender, ErrorEventArgs e)
    {
        _logger.LogError(e.GetException(), "[HotReload] 文件监听错误");
    }

    private void ResetDebounceTimer()
    {
        lock (_lock)
        {
            _debounceTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _debounceTimer = new Timer(
                OnDebounceElapsed,
                null,
                DebounceMs,
                Timeout.Infinite);
        }
    }

    private void OnDebounceElapsed(object? state)
    {
        var changes = _pendingChanges.ToArray();
        _pendingChanges.Clear();

        _logger.LogWarning("[HotReload] ============================================");
        _logger.LogWarning("[HotReload] 检测到插件变更，执行热更新（不停服）");
        foreach (var c in changes)
            _logger.LogWarning("[HotReload]   {Change}", c);
        _logger.LogWarning("[HotReload] ============================================");

        // 重新扫描并加载所有插件 DLL
        foreach (var dll in Directory.GetFiles(PluginDir, "DShop.*Plugin.dll"))
        {
            // 在后台执行热更新，不阻塞当前请求
            _ = HotReloadPluginAsync(dll);
        }
    }

    // ======================== 清理 ========================

    private void Cleanup()
    {
        _watcher?.Dispose();
        _debounceTimer?.Dispose();

        // 清理所有待卸载的旧插件
        foreach (var kvp in _pendingUnload)
        {
            kvp.Key.Loader?.Dispose();
            if (kvp.Key.ServiceProvider is IDisposable d)
                d.Dispose();
        }
        _pendingUnload.Clear();

        // 清理所有已加载的插件
        foreach (var plugin in _plugins.Values)
        {
            plugin.Loader?.Dispose();
            if (plugin.ServiceProvider is IDisposable d)
                d.Dispose();
        }
        _plugins.Clear();

        // 清理旧影子目录（保留最近 3 个版本）
        try
        {
            var dirs = Directory.GetDirectories(ShadowDir)
                .OrderByDescending(d => d)
                .Skip(3);
            foreach (var dir in dirs)
                Directory.Delete(dir, recursive: true);
        }
        catch { }
    }

    // ======================== 工具方法 ========================

    private static string GetFileVersion(string dllPath)
    {
        try
        {
            return FileVersionInfo.GetVersionInfo(dllPath).FileVersion ?? "未知";
        }
        catch
        {
            return "未知";
        }
    }

    // ======================== 内部类型 ========================

    /// <summary>
    /// 当前加载的插件上下文
    /// </summary>
    public class PluginContext
    {
        public required PluginLoader Loader { get; init; }
        public required Assembly Assembly { get; init; }
        /// <summary>
        /// 通过 ApplicationPartFactory 发现的所有 MVC 部件（控制器、视图等）
        /// 使用 McMaster.NETCore.Plugins.Mvc 内部相同的发现机制
        /// </summary>
        public required IReadOnlyList<ApplicationPart> ApplicationParts { get; init; }
        /// <summary>插件包装后的 ServiceProvider（回退到主根容器）</summary>
        public required IServiceProvider ServiceProvider { get; init; }
        /// <summary>插件内部纯 ServiceProvider（不含主容器服务，配合请求作用域联合使用）</summary>
        public required IServiceProvider InnerProvider { get; init; }
        /// <summary>插件服务注册信息（用于在构造失败时查找实现类型）</summary>
        public required IReadOnlyList<ServiceDescriptor> ServiceRegistrations { get; init; }
        public required Type? EntryType { get; init; }
        /// <summary>当前正在处理该插件控制器的活跃请求数（用于判断何时可安全卸载）</summary>
        public volatile int ActiveRequestCount;
    }

    /// <summary>
    /// 控制器服务解析器：递归处理整个依赖链，每个节点都尝试从三个来源查找：
    ///   1. 请求作用域（scoped 服务，如 DbContext）
    ///   2. 插件 DI（插件特有服务，如 IIdentityCommandService）
    ///   3. 主根容器（回退，如 IConfiguration）
    /// 解决插件服务依赖主容器服务时构造失败的问题（如 IdentityCommandService → DatabaseContext）
    /// </summary>
    private class PluginAwareServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _requestServices;
        private readonly IServiceProvider _pluginWrapped;
        private readonly IReadOnlyList<ServiceDescriptor> _registrations;

        public PluginAwareServiceProvider(
            IServiceProvider requestServices,
            IServiceProvider pluginWrapped,
            IReadOnlyList<ServiceDescriptor> registrations)
        {
            _requestServices = requestServices;
            _pluginWrapped = pluginWrapped;
            _registrations = registrations;
        }

        public object? GetService(Type serviceType)
        {
            // 1. 请求作用域优先（scoped 服务如 DatabaseContext 从这里拿）
            var service = _requestServices.GetService(serviceType);
            if (service != null) return service;

            // 2. 查插件注册信息，找到实现类型则手动构造（递归使用本 provider 兜底依赖）
            var descriptor = _registrations.FirstOrDefault(d => d.ServiceType == serviceType);
            if (descriptor?.ImplementationType != null)
            {
                return ActivatorUtilities.CreateInstance(this, descriptor.ImplementationType);
            }
            if (descriptor?.ImplementationFactory != null)
            {
                return descriptor.ImplementationFactory(this);
            }
            if (descriptor?.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            // 3. 包装容器回退（主根容器）
            return _pluginWrapped.GetService(serviceType);
        }
    }

    /// <summary>
    /// 包装插件 IServiceProvider，每次获取服务时延迟获取主容器的引用
    /// 避免在应用启动阶段主容器还未就绪时出现问题
    /// </summary>
    private class PluginScopedServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _inner;
        private readonly Func<IServiceProvider> _mainProviderFactory;

        public PluginScopedServiceProvider(
            IServiceProvider inner,
            Func<IServiceProvider> mainProviderFactory)
        {
            _inner = inner;
            _mainProviderFactory = mainProviderFactory;
        }

        public object? GetService(Type serviceType)
        {
            var service = _inner.GetService(serviceType);
            if (service != null) return service;
            var main = _mainProviderFactory();
            return main?.GetService(serviceType);
        }
    }
}
