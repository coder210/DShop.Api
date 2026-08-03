using DShop.BasePlugin.Services;
using DShop.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.BasePlugin;

public static class BasePluginService
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Identity services
        services.AddScoped<IIdentityQueryService, IdentityQueryService>();
        services.AddScoped<IIdentityCommandService, IdentityCommandService>();

        // System services
        services.AddScoped<ISystemQueryService, SystemQueryService>();
        services.AddScoped<ISystemCommandService, SystemCommandService>();
    }
}
