using MassTransit;
using Toolkit.Mapper;
using Toolkit.Interfaces;
using Toolkit.Exceptions;
using Benefit.Service.Infra;
using Toolkit.MessageBroker;
using Benefit.Domain.Interfaces;
using Benefit.Service.Sagas.Beneficiary.Contract;

namespace Benefit.Service.Workers;

public sealed class BeneficiaryTheAudioDbConsumer : BrokerConsumer<BeneficiaryImdbIntegrated>
{
    public BeneficiaryTheAudioDbConsumer(IPublishEndpoint publisher, IBenefitRepository benefitRepository)
    {
        _Publisher = publisher;
        _BenefitRepository = benefitRepository;
        _Mapper = MapperFactory.Map<BeneficiaryImdbIntegrated, BeneficiaryTheAudioDbIntegrated>();
    }

    private readonly IPublishEndpoint _Publisher;
    private readonly IBenefitRepository _BenefitRepository;
    private readonly IGenericMapper _Mapper;
    protected override async Task<BrokerConsumerResult> ConsumeAsync(BeneficiaryImdbIntegrated message)
    {
        if (message == null)
            throw new ArgumentNullException("Invalid message received as argument.");
        var benefit = await _BenefitRepository.GetByCPF(message.CPF);
        if (benefit == null)
            throw new NotFoundException($"No beneficiary found with CPF=\"{message.CPF}\".");
        //var imdbPerson = _ApiClient.GetPerson(BenefitContext.ImdbKey, benefit.Name).Result;
        //if (imdbPerson == null || imdbPerson.results == null || imdbPerson.results.Count == 0)
        //    return Sucess();
        //benefit.Works = imdbPerson.results
        //    .Select(o => new Work(o.title, o.image, o.description)).ToArray();
        var repo = (BenefitRepository)_BenefitRepository;
        repo.Context.Update(benefit);
        var evt = _Mapper.Map<BeneficiaryImdbIntegrated, BeneficiaryTheAudioDbIntegrated>(message);
        await _Publisher.Publish(evt);
        await repo.Context.SaveChangesAsync();
        return Sucess();
    }
}