using MassTransit;
using Toolkit.Mapper;
using Toolkit.Interfaces;
using Toolkit.Exceptions;
using Toolkit.MessageBroker;
using Benefit.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Benefit.Service.Sagas.Beneficiary.Contract;

namespace Benefit.Service.Workers;

public sealed class BeneficiaryNotifyFinishConsumer : BrokerConsumer<BeneficiaryTheAudioDbIntegrated>
{
    public BeneficiaryNotifyFinishConsumer(IPublishEndpoint publisher, ILogger<BeneficiaryNotifyFinishConsumer> logger, IBenefitRepository benefitRepository)
    {
        _Logger = logger;
        _Publisher = publisher;
        _BenefitRepository = benefitRepository;
        _Mapper = MapperFactory.Map<BeneficiaryTheAudioDbIntegrated, BeneficiaryNotifyFinished>();
    }

    private readonly ILogger _Logger;
    private readonly IPublishEndpoint _Publisher;
    private readonly IBenefitRepository _BenefitRepository;
    private readonly IGenericMapper _Mapper;

    protected override ILogger Logger => _Logger;

    protected override async Task ConsumeAsync(BeneficiaryTheAudioDbIntegrated message)
    {
        if (message == null)
            throw new ArgumentNullException("Invalid message received as argument.");
        var benefit = await _BenefitRepository.GetByCPF(message.CPF);
        if (benefit == null)
            throw new NotFoundException($"No beneficiary found with CPF=\"{message.CPF}\".");
        _Logger.LogInformation("BeneficiaryNotifyFinishConsumer executed.");
        var evt = _Mapper.Map<BeneficiaryTheAudioDbIntegrated, BeneficiaryNotifyFinished>(message);
        await _Publisher.Publish(evt);
    }
}