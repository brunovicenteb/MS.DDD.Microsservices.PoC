using Refit;
using MassTransit;
using Toolkit.Register;
using Benefit.Service.Infra;
using Benefit.Domain.Interfaces;
using Benefit.Service.APIs.Imdb;
using Microsoft.Extensions.DependencyInjection;

namespace Benefit.Service.Workers;

public sealed class BenefitConsumerFactory : ResourcesFactory
{
    public static string ImdbKey;
    private const string _ImdbKeyVariableName = "IMDB_API_KEY";
    private const string _ImdbUrlApi = "https://imdb-api.com/";

    protected override void DeclareConsumers()
    {
        AddConsumer<BenefitInsertedConsumer>();
        AddConsumer<BenefitLoadMoviesConsumer>();
    }

    protected override void RegisterResources(IServiceCollection serviceColllection, IBusRegistrationConfigurator busRegistration)
    {
        base.RegisterResources(serviceColllection, busRegistration);
        serviceColllection.AddScoped<IBenefitRepository, BenefitRepository>();
        ImdbKey = Environment.GetEnvironmentVariable(_ImdbKeyVariableName);
        serviceColllection.AddRefitClient<IImdbApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(_ImdbUrlApi));
    }
}