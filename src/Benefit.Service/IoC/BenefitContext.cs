using Refit;
using MassTransit;
using Benefit.Service.Infra;
using Benefit.Domain.Benefit;
using Benefit.Service.Workers;
using Benefit.Service.APIs.Imdb;
using Benefit.Domain.Interfaces;
using Toolkit.TransactionalOutBox;
using Microsoft.EntityFrameworkCore;
using Benefit.Service.APIs.TheAudioDb;
using Benefit.Service.Sagas.Beneficiary;
using Microsoft.Extensions.DependencyInjection;
using Benefit.Service.Sagas.Beneficiary.Contract;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefit.Service.IoC;

public class BenefitContext : OutBoxDbContext
{
    public static string ImdbKey
        => _ImdbKey;
    private static string _ImdbKey;
    private const string _ImdbUrlApi = "https://imdb-api.com/";
    private const string _ImdbKeyVariableName = "IMDB_API_KEY";
    private const string _TheAudioDbUrlApi = "https://www.theaudiodb.com/";

    public BenefitContext()
    {
    }

    public BenefitContext(DbContextOptions<BenefitContext> options)
        : base(options)
    {
    }

    public DbSet<Beneficiary> Beneficiaries { get; set; }
    public DbSet<ImdbWork> ImdbWork { get; set; }
    public DbSet<TheAudioDbWork> TheAudioDbWork { get; set; }

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
        services.AddRefitClient<ITheAudioDbApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(_TheAudioDbUrlApi));
        busRegistration.AddConsumer<BenefiteImdbConsumer>();
        busRegistration.AddConsumer<BeneficiaryTheAudioDbConsumer>();
        busRegistration.AddConsumer<BeneficiaryNotifyFinishConsumer>();
        var sagaStateMachine = busRegistration.AddSagaStateMachine<BeneficiaryStateMachine, BeneficiaryState>(cfg =>
        {
            if (DbType == DatabaseType.InMemory)
                cfg.UseInMemoryOutbox();
        });
        if (DbType == DatabaseType.InMemory)
            sagaStateMachine.InMemoryRepository();
        else
        {
            sagaStateMachine.EntityFrameworkRepository(r =>
            {
                r.ExistingDbContext<BenefitContext>();
                if (DbType == DatabaseType.SqlServer)
                    r.UseSqlServer();
                else
                    r.UsePostgres();
            });
        }
    }

    protected override void DoModelCreating(ModelBuilder modelBuilder)
    {
        base.DoModelCreating(modelBuilder);
        MapBeneficiary(modelBuilder);
        MapImdbWork(modelBuilder);
        MapTheAudioDbWork(modelBuilder);
    }

    private void MapTheAudioDbWork(ModelBuilder modelBuilder)
    {
        var registration = modelBuilder.Entity<TheAudioDbWork>();
        registration.HasKey(e => e.ID);
    }

    private void MapImdbWork(ModelBuilder modelBuilder)
    {
        var registration = modelBuilder.Entity<ImdbWork>();
        registration.HasKey(e => e.ID);
        registration.Property(e => e.Title)
                .HasMaxLength(150)
                .IsRequired();
    }

    private void MapBeneficiary(ModelBuilder modelBuilder)
    {
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