using MassTransit;
using Toolkit.Mapper;
using Toolkit.Interfaces;
using Toolkit.Exceptions;
using Toolkit.MessageBroker;
using Benefit.Domain.Benefit;
using Benefit.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Benefit.Service.APIs.TheAudioDb;
using Benefit.Service.APIs.TheAudioDb.DTO;
using Benefit.Service.Sagas.Beneficiary.Contract;
using Toolkit;

namespace Benefit.Service.Workers;

public sealed class BeneficiaryTheAudioDbConsumer : BrokerConsumer<BeneficiaryImdbIntegrated>
{
    public BeneficiaryTheAudioDbConsumer(IPublishEndpoint publisher, ILogger<BeneficiaryTheAudioDbConsumer> logger, ITheAudioDbApiClient apiClient,
        IBenefitRepository benefitRepository)
    {
        _Logger = logger;
        _Publisher = publisher;
        _BenefitRepository = benefitRepository;
        _ApiClient = apiClient;
        _Mapper = MapperFactory.Map<BeneficiaryImdbIntegrated, BeneficiaryTheAudioDbIntegrated>();
    }

    private readonly ILogger _Logger;
    private readonly IPublishEndpoint _Publisher;
    private readonly IBenefitRepository _BenefitRepository;
    private readonly IGenericMapper _Mapper;
    private readonly ITheAudioDbApiClient _ApiClient;

    protected override ILogger Logger => _Logger;

    protected override async Task ConsumeAsync(BeneficiaryImdbIntegrated message)
    {
        if (message == null)
            throw new ArgumentNullException("Invalid message received as argument.");
        var benefit = await _BenefitRepository.GetByCPF(message.CPF);
        if (benefit == null)
            throw new NotFoundException($"No beneficiary found with CPF=\"{message.CPF}\".");
        var apiResponse = await TryExecute(async () => await _ApiClient.Search(benefit.Name),
            $"Unable to consume TheAudioDb API for beneficiary \"{benefit.Name}\" even after {RetryCount} attempts.");
        if (apiResponse != null)
            await SaveTheAudioDbWorks(benefit, apiResponse);
        await _BenefitRepository.UpdateAsync(benefit);
        var evt = _Mapper.Map<BeneficiaryImdbIntegrated, BeneficiaryTheAudioDbIntegrated>(message);
        await _Publisher.Publish(evt);
        await _BenefitRepository.SaveChangesAsync();
    }

    private async Task<bool> SaveTheAudioDbWorks(Beneficiary benefit, TheAudioDbResponse apiResponse)
    {
        if (apiResponse.artists == null || apiResponse.artists.Count == 0)
        {
            _Logger.LogWarning($"No work found on TheAudioDbApi for the name \"{benefit.Name}\".");
            return false;
        }
        var works = apiResponse.artists.Select(o => new TheAudioDbWork(o.idArtist, o.strArtist, o.strArtistAlternate,
            o.strLabel, o.intFormedYear, o.intBornYear, o.strDiedYear, o.strStyle, o.strGenre, o.strWebsite, o.strFacebook, o.strTwitter, benefit.ID)).ToArray();
        await _BenefitRepository.AddTheAudioDbWorkAsync(works, false);
        return true;
    }
}