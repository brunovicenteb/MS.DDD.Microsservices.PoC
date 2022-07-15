using Toolkit.Interfaces;
using Benefit.Domain.Benefit;

namespace Benefit.Domain.Interfaces;

public interface IBenefitRepository : IBaseRepository<Beneficiary>
{
    Task<Beneficiary> GetByCPF(string cpf);
}