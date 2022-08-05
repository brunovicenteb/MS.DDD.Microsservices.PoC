using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
public class ObjectStateMapMock : SagaClassMap<ObjectStateMock>
{
    protected override void Configure(EntityTypeBuilder<ObjectStateMock> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState);
        entity.Property(x => x.Name);
    }
}