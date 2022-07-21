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

    public async Task<bool> AddImdbWorksAsync(ImdbWork[] imdbWorks, bool applySave = true)
    {
        int n = imdbWorks.SafeLength();
        for (int i = 0; i < n; i++)
        {
            var imdbWork = imdbWorks[i];
            await Context.ImdbWork.AddAsync(imdbWork);
        }
        if (applySave)
            return await Context.SaveChangesAsync() > 0;
        return true;
    }

    public async Task<bool> AddTheAudioDbWorkAsync(TheAudioDbWork[] theAudioDbWorks, bool applySave = true)
    {
        int n = theAudioDbWorks.SafeLength();
        for (int i = 0; i < n; i++)
        {
            var theAudioDbWork = theAudioDbWorks[i];
            await Context.TheAudioDbWork.AddAsync(theAudioDbWork);
        }
        if (applySave)
            return await Context.SaveChangesAsync() > 0;
        return true;
    }
    
    public override async Task<Beneficiary> GetObjectByIDAsync(int id)
    {
        return await Collection
            .Include(o => o.ImdbWorks)
            .Include(p => p.TheAudioDbWorks)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.ID == id);
    }

    public override async Task<IEnumerable<Beneficiary>> GetAsync(int limit, int start)
    {
        return await Collection
            .Include(o => o.ImdbWorks)
            .Include(p => p.TheAudioDbWorks)
            .OrderBy(o => o.CreateAt)
            .Skip(start)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Beneficiary> GetByCPF(string cpf)
    {
        return await Collection
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.CPF == cpf);
    }
}