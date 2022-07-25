using MassTransit;

namespace Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
public class ObjectStateMock : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public string Name { get; set; }
}