using Toolkit.Mongo;
using Benefit.Domain.Benefit;
using Benefit.Domain.Interfaces;

namespace Benefit.Service.Infra;

public sealed class BenefitRepository : BaseRepository<Beneficiary>, IBenefitRepository
{
    protected override string CollectionName => "Benefit";
}