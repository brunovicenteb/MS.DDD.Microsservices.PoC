using Benefit.Service.IoC;
using Toolkit.RelationalDb;
using Benefit.Domain.Benefit;
using Benefit.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Benefit.Service.Infra;

public sealed class BenefitRepository : RelationalDbRepository<BenefitContext, Beneficiary>, IBenefitRepository
{
    public BenefitRepository(BenefitContext context)
        : base(context)
    {
    }

    protected override DbSet<Beneficiary> Collection => Context.Beneficiaries;
}