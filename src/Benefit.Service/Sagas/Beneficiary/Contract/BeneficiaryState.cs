using MassTransit;

namespace Benefit.Service.Sagas.Beneficiary.Contract;

public class BeneficiaryState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
}