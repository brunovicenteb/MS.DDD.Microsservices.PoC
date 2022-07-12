using Benefit.Domain.Benefit;
using Microsoft.EntityFrameworkCore;
using Toolkit.TransactionalOutBox;

namespace Benefit.Service.IoC;
public class BenefitContext : TransactionalOutBoxDbContext
{
    public BenefitContext(DbContextOptions<BenefitContext> options)
        : base(options)
    {
    }

    public DbSet<Beneficiary> Beneficiaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Beneficiary>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.HasIndex(e => e.CPF)
                .IsUnique();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.CPF)
                .HasMaxLength(11)
                .IsRequired();
            entity.Property(e => e.CreateAt)
                .IsRequired()
                .HasDefaultValueSql("getutcdate()");
        });
        base.OnModelCreating(modelBuilder);
    }
}