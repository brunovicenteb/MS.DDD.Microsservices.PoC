using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Toolkit.MessageBroker.TransactionOutbox;

public class RegistrationStateMap : SagaClassMap<RegistrationState>
{
    protected override void Configure(EntityTypeBuilder<RegistrationState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState);
        entity.Property(x => x.RegistrationDate);
        entity.Property(x => x.EventId);
        entity.Property(x => x.MemberId);
    }
}