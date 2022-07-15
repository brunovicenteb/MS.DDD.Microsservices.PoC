using Toolkit;
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

    public async Task<Beneficiary> GetByCPF(string cpf)
    {
        if (cpf.IsEmpty())
            throw new ArgumentNullException($"Empty value received as argument CPF.");
        if (!cpf.IsValidCPF())
            throw new ArgumentNullException($"Invalid CPF value \"{cpf}\" received as argument.");
        return await Collection
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.CPF == cpf);
    }
}