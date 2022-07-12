using MassTransit;

namespace Toolkit.MessageBroker.TransactionOutbox;

public class RegistrationState : SagaStateMachineInstance
{
    public string CurrentState { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public string EventId { get; set; } = null!;
    public string MemberId { get; set; } = null!;
    public Guid CorrelationId { get; set; }
}