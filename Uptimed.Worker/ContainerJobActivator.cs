using Hangfire;

namespace Uptimed.Worker;

public class ContainerJobActivator(IServiceProvider container) : JobActivator
{
    public override object? ActivateJob(Type type)
    {
        return container.GetService(type);
    }
}