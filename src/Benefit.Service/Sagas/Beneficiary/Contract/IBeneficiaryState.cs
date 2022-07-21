namespace Benefit.Service.Sagas.Beneficiary.Contract;

public record IBeneficiaryState
{
    public Guid CorrelationId { get; set; }
    public string Name { get; set; }
    public string CPF { get; set; }
}