using MassTransit;
using Benefit.Service.Infra;
using Toolkit.MessageBroker;
using Benefit.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Benefit.Service.Workers;

public sealed class BenefitConsumerFactory : BrokerConsumerFactory
{
    protected override void DeclareConsumers()
    {
        AddConsumer<BenefitInsertedConsumer>();
    }

    protected override void RegisterResources(IServiceCollection serviceColllection, IBusRegistrationConfigurator busRegistration)
    {
        base.RegisterResources(serviceColllection, busRegistration);
        serviceColllection.AddScoped<IBenefitRepository, BenefitRepository>();
    }
}