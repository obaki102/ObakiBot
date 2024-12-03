using Microsoft.SemanticKernel;

namespace ObakiBot.Ai.Services;

public abstract class AiServiceBase
{
    protected Kernel Kernel { get; }

    protected AiServiceBase(Kernel kernel)
    {
        Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }

    protected void LoadPlugin<TPlugin>(string name, IServiceProvider serviceProvider) where TPlugin : class
    {
        Kernel.Plugins.AddFromType<TPlugin>(name, serviceProvider);
    }
}
