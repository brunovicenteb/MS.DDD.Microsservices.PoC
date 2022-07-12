using Benefit.Domain.Benefit;
using Benefit.Domain.Interfaces;
using Benefit.Service.IoC;
using Microsoft.EntityFrameworkCore;
using Toolkit.RelationalDb;

namespace Benefit.Service.Infra;

public sealed class BenefitRepository : RelationalDbRepository<BenefitContext, Beneficiary>, IBenefitRepository
{
    public BenefitRepository(BenefitContext context)
        : base(context)
    {
    }

    protected override DbSet<Beneficiary> Collection => Context.Beneficiaries;
}