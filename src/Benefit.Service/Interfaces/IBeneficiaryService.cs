using Benefit.Domain.Benefit;

namespace Benefit.Service.Interfaces;

public interface IBeneficiaryService
{
    Task<Beneficiary> SubmitBeneficiary(OperatorType operatorType, string name, string cpf, DateTime? birthDate);
}