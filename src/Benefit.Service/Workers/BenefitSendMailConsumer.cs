using Toolkit.MessageBroker;
using Microsoft.Extensions.Logging;
using Benefit.Domain.AggregatesModel.Benefit;

namespace Benefit.Service.Workers;

public sealed class BenefitSendMailConsumer : BrokerConsumer<BeneficiaryRegistered>
{
    public BenefitSendMailConsumer(ILogger<BenefitLoadMoviesConsumer> logger)
    {
        _Logger = logger;
    }

    private readonly ILogger<BenefitLoadMoviesConsumer> _Logger;
    protected override async Task<BrokerConsumerResult> ConsumeAsync(BeneficiaryRegistered message)
    {
        await Task.CompletedTask;
        _Logger.LogWarning("Benefit--Send--Mail--Consumer--Running.");
        return Sucess();
    }
}