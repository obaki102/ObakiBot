using Microsoft.Extensions.DependencyInjection;

namespace ObakiBot.Core;

public static class ServiceLocator
{
    private static IServiceProvider? _serviceProvider;

    public static void Configure(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static T GetService<T>() where T : class
    {
        return _serviceProvider?.GetRequiredService<T>() ?? throw new InvalidOperationException("Service provider not configured");
    }
}
