using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract.States;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Toolkit.Interfaces;
using Toolkit.Mapper;
using Toolkit.MessageBroker;

namespace Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Workers;
/*public class ObjectCreatedConsumerMock : BrokerConsumer<ObjectCreatedMock>
{
    public ObjectCreatedConsumerMock(IPublishEndpoint publisher, ILogger<ObjectCreatedConsumerMock> logger)
    {
        _Publisher = publisher;
        _Logger = logger;
        _Mapper = MapperFactory.Map<BeneficiaryRegistered, BeneficiaryImdbIntegrated>();
    }

    private readonly ILogger _Logger;
    private readonly IPublishEndpoint _Publisher;
    private readonly IGenericMapper _Mapper;

    protected override ILogger Logger => _Logger;

    protected override async Task ConsumeAsync(BeneficiaryRegistered message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));
        var benefit = await _BenefitRepository.GetByCPF(message.CPF);
        if (benefit == null)
            throw new NotFoundException($"No beneficiary found with CPF=\"{message.CPF}\".");
        var apiResponse = await TryExecute(async () => await _ApiClient.GetPerson(BenefitContext.ImdbKey, benefit.Name),
            $"Unable to consume Imdb API for beneficiary \"{benefit.Name}\" even after five attempts.");
        if (apiResponse != null)
            await SaveImdbWorks(benefit, apiResponse);
        var evt = _Mapper.Map<BeneficiaryRegistered, BeneficiaryImdbIntegrated>(message);
        await _Publisher.Publish(evt);
        await _BenefitRepository.SaveChangesAsync();
    }
}*/