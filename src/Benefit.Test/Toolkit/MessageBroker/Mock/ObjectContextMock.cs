using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Toolkit.TransactionalOutBox;

namespace Benefit.Test.Toolkit.MessageBroker.Mock;
public class ObjectContextMock : OutBoxDbContext
{
    public ObjectContextMock()
    {
    }

    public ObjectContextMock(DbContextOptions<ObjectContextMock> options)
        : base(options)
    {
    }

    public DbSet<ObjectMock> objects { get; set; }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new ObjectStateMapMock(); }
    }

    public override void RegisterConsumers(IServiceCollection services, IBusRegistrationConfigurator busRegistration)
    {
        //busRegistration.AddConsumer<BeneficiaryNotifyFinishConsumer>();
        //busRegistration.AddConsumer<BeneficiaryNotifyFinishConsumer>();
        //busRegistration.AddSagaStateMachine<ObjectStateMachineMock, ObjectStateMock, ObjectStateDefinitionMock>()
        //    .EntityFrameworkRepository(r =>
        //    {
        //        r.ExistingDbContext<ObjectContextMock>();
        //            r.Use();
        //    });
    }
}