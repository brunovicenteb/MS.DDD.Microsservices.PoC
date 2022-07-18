using MassTransit;
using Benefit.Service.IoC;

namespace Benefit.Service.Sagas.Beneficiary.Contract;

public class BeneficiaryStateDefinition : SagaDefinition<BeneficiaryState>
{
    private readonly IServiceProvider _Provider;

    public BeneficiaryStateDefinition(IServiceProvider provider)
    {
        _Provider = provider;
    }
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<BeneficiaryState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000, 1000, 1000, 1000, 1000));
        endpointConfigurator.UseEntityFrameworkOutbox<BenefitContext>(_Provider);
    }
}