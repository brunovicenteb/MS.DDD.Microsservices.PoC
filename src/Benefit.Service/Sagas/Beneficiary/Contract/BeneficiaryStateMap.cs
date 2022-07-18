using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefit.Service.Sagas.Beneficiary.Contract;

public class BeneficiaryStateMap : SagaClassMap<BeneficiaryState>
{
    protected override void Configure(EntityTypeBuilder<BeneficiaryState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState);
        entity.Property(x => x.Name);
        entity.Property(x => x.CPF);
    }
}