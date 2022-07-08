using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.Register;

public abstract class ResourcesFactory
{
    private readonly List<Type> _Consumers = new List<Type>();

    protected abstract void DeclareConsumers();

    protected void AddConsumer<T>()
        where T : class, IBrokerConsumer
        => _Consumers.Add(typeof(T));

    public void RegisterConsumers(IServiceCollection serviceColllection, IBusRegistrationConfigurator busRegistration)
    {
        DeclareConsumers();
        foreach (var consumer in _Consumers)
            busRegistration.AddConsumer(consumer);
        RegisterResources(serviceColllection, busRegistration);
    }

    protected virtual void RegisterResources(IServiceCollection serviceColllection, IBusRegistrationConfigurator busRegistration)
    {
    }
}