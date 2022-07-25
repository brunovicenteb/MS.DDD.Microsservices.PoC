using MassTransit;

namespace Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
public class ObjectStateDefinitionMock : SagaDefinition<ObjectStateMock>
{
    private readonly IServiceProvider _Provider;

    public ObjectStateDefinitionMock(IServiceProvider provider)
    {
        _Provider = provider;
    }
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<ObjectStateMock> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<ObjectContextMock>(_Provider);
    }
}