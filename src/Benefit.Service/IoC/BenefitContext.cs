using Benefit.Domain.Benefit;
using Toolkit.TransactionalOutBox;
using Microsoft.EntityFrameworkCore;
using Benefit.Domain.AggregatesModel.Benefit;
using MassTransit.EntityFrameworkCoreIntegration;

namespace Benefit.Service.IoC;

public class BenefitContext : TransactionalOutBoxDbContext
{
    public BenefitContext(DbContextOptions<BenefitContext> options)
        : base(options)
    {
    }

    public DbSet<Beneficiary> Beneficiaries { get; set; }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new BeneficiaryStateMap(); }
    }

    protected override void DoModelCreating(ModelBuilder modelBuilder)
    {
        base.DoModelCreating(modelBuilder);
        var registration = modelBuilder.Entity<Beneficiary>();
        registration.HasKey(e => e.ID);
        registration.HasIndex(e => e.CPF)
                .IsUnique();
        registration.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();
        registration.Property(e => e.CPF)
                .HasMaxLength(11)
                .IsRequired();
        registration.Property(e => e.CreateAt)
                .IsRequired()
                .HasDefaultValueSql("getutcdate()");
    }
}