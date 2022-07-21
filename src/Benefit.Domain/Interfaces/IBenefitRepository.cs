using Toolkit.Interfaces;
using Benefit.Domain.Benefit;

namespace Benefit.Domain.Interfaces;

public interface IBenefitRepository : IBaseRepository<Beneficiary>
{
    Task<Beneficiary> GetByCPF(string cpf);

    Task<bool> AddImdbWorksAsync(ImdbWork[] imdbWorks, bool applySave = true);

    Task<bool> AddTheAudioDbWorkAsync(TheAudioDbWork[] theAudioDbWorks, bool applySave = true);
}