using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MassTransit.EntityFrameworkCoreIntegration;

namespace Toolkit.TransactionalOutBox;

public abstract class OutBoxDbContext : SagaDbContext
{
    public static DatabaseType DbType
        => _DbType;
    private static DatabaseType _DbType;

    internal static void SetDbType(DatabaseType dbType)
    {
        _DbType = dbType;
    }

    public OutBoxDbContext()
        : this(new DbContextOptions<OutBoxDbContext>())
    {
    }

    public OutBoxDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public virtual void RegisterConsumers(IServiceCollection services, IBusRegistrationConfigurator busRegistration)
    {
    }

    protected virtual void DoModelCreating(ModelBuilder modelBuilder)
    {
    }

    protected override sealed void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        DoModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}