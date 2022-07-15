using Refit;
using MassTransit;
using Benefit.Service.Infra;
using Benefit.Domain.Benefit;
using Benefit.Service.APIs.Imdb;
using Benefit.Domain.Interfaces;
using Toolkit.TransactionalOutBox;
using Microsoft.EntityFrameworkCore;
using Benefit.Domain.AggregatesModel.Benefit;
using Microsoft.Extensions.DependencyInjection;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Benefit.Service.Workers;

namespace Benefit.Service.IoC;

public class BenefitContext : OutBoxDbContext
{
    public static string ImdbKey
        => _ImdbKey;
    private static string _ImdbKey;
    private const string _ImdbKeyVariableName = "IMDB_API_KEY";
    private const string _ImdbUrlApi = "https://imdb-api.com/";

    public BenefitContext()
    {
    }

    public BenefitContext(DbContextOptions<BenefitContext> options)
        : base(options)
    {
    }

    public DbSet<Beneficiary> Beneficiaries { get; set; }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new BeneficiaryStateMap(); }
    }

    public override void RegisterConsumers(IServiceCollection services, IBusRegistrationConfigurator busRegistration)
    {
        base.RegisterConsumers(services, busRegistration);
        services.AddScoped<IBenefitRepository, BenefitRepository>();
        _ImdbKey = Environment.GetEnvironmentVariable(_ImdbKeyVariableName);
        services.AddRefitClient<IImdbApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(_ImdbUrlApi));
        busRegistration.AddConsumer<BenefitLoadMoviesConsumer>();
        busRegistration.AddConsumer<BenefitSendMailConsumer>();
        busRegistration.AddSagaStateMachine<BeneficiaryStateMachine, BeneficiaryState, BeneficiaryStateDefinition>()
            .EntityFrameworkRepository(r =>
            {
                r.ExistingDbContext<BenefitContext>();
                if (DbType == DatabaseType.SqlServer)
                    r.UseSqlServer();
                else
                    r.UsePostgres();
            });
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
                .IsRequired();
        if (DbType == DatabaseType.SqlServer)
            DoModelCreateSqlServer(registration);
        else
            DoModelCreatePostgress(modelBuilder, registration);
    }

    private void DoModelCreateSqlServer(EntityTypeBuilder<Beneficiary> registration)
    {
        registration.Property(e => e.CreateAt)
            .HasDefaultValueSql("getutcdate()");
    }

    private void DoModelCreatePostgress(ModelBuilder modelBuilder, EntityTypeBuilder<Beneficiary> registration)
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