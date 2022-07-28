using Benefit.Test.Toolkit.MessageBroker.Mock.Infra;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Workers;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        base.RegisterConsumers(services, busRegistration);
        services.AddScoped<IObjectRepositoryMock, ObjectRepositoryMock>();
        busRegistration.AddConsumer<ObjectCreatedConsumerMock>();
        busRegistration
            .AddSagaStateMachine<ObjectStateMachineMock, ObjectStateMock, ObjectStateDefinitionMock>().InMemoryRepository();
    }

    protected override void DoModelCreating(ModelBuilder modelBuilder)
    {
        base.DoModelCreating(modelBuilder);
        MapObjectMock(modelBuilder);
    }

    private void MapObjectMock(ModelBuilder modelBuilder)
    {
        var registration = modelBuilder.Entity<ObjectMock>();
        registration.HasKey(e => e.ID);
        registration.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();
        registration.Property(e => e.CreateAt)
                .IsRequired();

        if (DbType == DatabaseType.SqlServer)
            DoModelCreateSqlServer(registration);
        else
            DoModelCreatePostgress(modelBuilder, registration);
    }

    private void DoModelCreateSqlServer(EntityTypeBuilder<ObjectMock> registration)
    {
        registration.Property(e => e.CreateAt)
            .HasDefaultValueSql("getutcdate()");
    }

    private void DoModelCreatePostgress(ModelBuilder modelBuilder, EntityTypeBuilder<ObjectMock> registration)
    {
        modelBuilder.HasDefaultSchema("public");
        registration.Property(e => e.CreateAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd()
            .IsRequired();
        registration.Property(e => e.UpdateAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnUpdate();
    }
}