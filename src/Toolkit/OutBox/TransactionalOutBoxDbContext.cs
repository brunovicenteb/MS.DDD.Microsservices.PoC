using MassTransit;
using Microsoft.EntityFrameworkCore;
using MassTransit.EntityFrameworkCoreIntegration;

namespace Toolkit.TransactionalOutBox;

public abstract class TransactionalOutBoxDbContext : SagaDbContext
{
    public TransactionalOutBoxDbContext(DbContextOptions options)
        : base(options)
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