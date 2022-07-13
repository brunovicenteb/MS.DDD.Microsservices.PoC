using Toolkit.MessageBroker;
using Benefit.Domain.Interfaces;
using Benefit.Service.APIs.Imdb;
using Benefit.Domain.AggregatesModel.Benefit;

namespace Benefit.Service.Workers;

public sealed class BenefitLoadMoviesConsumer : BrokerConsumer<BeneficiaryImdbIntegrated>
{
    public BenefitLoadMoviesConsumer(IImdbApiClient apiClient, IBenefitRepository benefitRepository)
    {
        _ApiClient = apiClient;
        _BenefitRepository = benefitRepository;
    }

    private readonly IImdbApiClient _ApiClient;
    private readonly IBenefitRepository _BenefitRepository;

    protected override async Task<BrokerConsumerResult> ConsumeAsync(BeneficiaryImdbIntegrated message)
    {
        var benefit = await _BenefitRepository.GetObjectByIDAsync(message.ID);
        var imdbPerson = _ApiClient.GetPerson(BenefitConsumerFactory.ImdbKey, benefit.Name).Result;
        if (imdbPerson == null || imdbPerson.results == null || imdbPerson.results.Count == 0)
            return Sucess();
        //benefit.Works = imdbPerson.results
        //    .Select(o => new Work(o.title, o.image, o.description)).ToArray();
        await _BenefitRepository.UpdateAsync(benefit);
        return Sucess();
    }
}