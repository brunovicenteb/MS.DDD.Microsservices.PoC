using Serilog;
using MassTransit;
using Serilog.Events;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.TransactionalOutBox;

internal class TransactionalOutBox<T> : ILogable, IOpenTelemetreable, IDatabaseable, IBrokeable where T : TransactionalOutBoxDbContext
{
    internal TransactionalOutBox(WebApplicationBuilder builder)
    {
        _Builder = builder;
    }

    private readonly WebApplicationBuilder _Builder;
    private Type _DbContext;

    public IBrokeable UseSqlServer(bool recreateDatabase)
    {
        _DbContext = typeof(T);
        _Builder.Services.AddDbContext<T>(x =>
        {
            var connectionString = "Server=localhost\\SQLEXPRESS;Database=OutboxTest;User Id=sa;Password=supersenha;";
            x.UseSqlServer(connectionString, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                options.MigrationsHistoryTable($"__{nameof(T)}");
                options.EnableRetryOnFailure(5);
                options.MinBatchSize(1);
            });
        });
        _Builder.Services.AddHostedService(o => new RecreateDatabaseHostedService<T>(recreateDatabase, o));
        return this;
    }

    public IBrokeable UsePostgres(bool recreateDatabase = true)
    {
        //_DbContext = typeof(T);
        //_Builder.Services.AddDbContext<T>(x =>
        //{
        //    var connectionString = "Host=localhost;Database=chamadosDB;Port=5432;Username=postgres;Password=supersenha";
        //    x.UseNpgsql(connectionString, options =>
        //    {
        //        options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
        //        options.MigrationsHistoryTable($"__{nameof(T)}");
        //        options.EnableRetryOnFailure(5);
        //        options.MinBatchSize(1);
        //    });
        //});
        //_Builder.Services.AddHostedService(o => new RecreateDatabaseHostedService<T>(recreateDatabase, o));
        return this;
    }

    public void UseRabbitMq()
    {
        _Builder.Services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<T>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UseSqlServer();
                o.UseBusOutbox();
            });
            x.UsingRabbitMq((_, cfg) =>
            {
                cfg.AutoStart = true;
            });
        });
    }

    public IOpenTelemetreable UseSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        _Builder.Host.UseSerilog();
        return this;
    }

    public IDatabaseable UseOpenTelemetry()
    {
        return this;
    }

    public IDatabaseable DoNotOpenTelemetry()
    {
        return this;
    }
}