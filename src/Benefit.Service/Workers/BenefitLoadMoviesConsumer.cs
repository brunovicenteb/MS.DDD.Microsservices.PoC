using Toolkit.Exceptions;
using Benefit.Service.IoC;
using Toolkit.MessageBroker;
using Benefit.Domain.Interfaces;
using Benefit.Service.APIs.Imdb;
using Benefit.Domain.AggregatesModel.Benefit;
using Microsoft.Extensions.Logging;

namespace Benefit.Service.Workers;

public sealed class BenefitLoadMoviesConsumer : BrokerConsumer<BeneficiaryRegistered>
{
    public BenefitLoadMoviesConsumer(IImdbApiClient apiClient, IBenefitRepository benefitRepository)
    {
        _ApiClient = apiClient;
        _BenefitRepository = benefitRepository;
    }

    private readonly IImdbApiClient _ApiClient;
    private readonly IBenefitRepository _BenefitRepository;

    protected override async Task<BrokerConsumerResult> ConsumeAsync(BeneficiaryRegistered message)
    {
        if (message == null)
            throw new ArgumentNullException("Invalid message received as argument.");
        var benefit = await _BenefitRepository.GetByCPF(message.CPF);
        if (benefit == null)
            throw new NotFoundException($"No beneficiary found with CPF=\"{message.CPF}\".");
        var imdbPerson = _ApiClient.GetPerson(BenefitContext.ImdbKey, benefit.Name).Result;
        if (imdbPerson == null || imdbPerson.results == null || imdbPerson.results.Count == 0)
            return Sucess();
        //benefit.Works = imdbPerson.results
        //    .Select(o => new Work(o.title, o.image, o.description)).ToArray();
        await _BenefitRepository.UpdateAsync(benefit);
        return Sucess();
    }
}