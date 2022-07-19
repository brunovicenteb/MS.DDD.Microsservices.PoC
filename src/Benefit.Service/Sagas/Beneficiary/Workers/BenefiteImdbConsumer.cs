using Toolkit;
using MassTransit;
using Toolkit.Mapper;
using Toolkit.Interfaces;
using Toolkit.Exceptions;
using Benefit.Service.IoC;
using Toolkit.MessageBroker;
using Benefit.Domain.Interfaces;
using Benefit.Service.APIs.Imdb;
using Benefit.Service.Sagas.Beneficiary.Contract;
using Benefit.Domain.Benefit;
using Benefit.Service.APIs.Imdb.DTO;
using Microsoft.Extensions.Logging;

namespace Benefit.Service.Workers;

public sealed class BenefiteImdbConsumer : BrokerConsumer<BeneficiaryRegistered>
{
    public BenefiteImdbConsumer(IPublishEndpoint publisher, ILogger<BenefiteImdbConsumer> logger, IImdbApiClient apiClient,
        IBenefitRepository benefitRepository)
    {
        _Publisher = publisher;
        _ApiClient = apiClient;
        _Logger = logger;
        _BenefitRepository = benefitRepository;
        _Mapper = MapperFactory.Map<BeneficiaryRegistered, BeneficiaryImdbIntegrated>();
    }

    private readonly ILogger _Logger;
    private readonly IImdbApiClient _ApiClient;
    private readonly IPublishEndpoint _Publisher;
    private readonly IBenefitRepository _BenefitRepository;
    private readonly IGenericMapper _Mapper;

    protected override async Task<BrokerConsumerResult> ConsumeAsync(BeneficiaryRegistered message)
    {
        if (message == null)
            throw new ArgumentNullException("Invalid message received as argument.");
        var benefit = await _BenefitRepository.GetByCPF(message.CPF);
        if (benefit == null)
            throw new NotFoundException($"No beneficiary found with CPF=\"{message.CPF}\".");
        //var imdbPerson = _ApiClient.GetPerson(BenefitContext.ImdbKey, benefit.Name.ToEscapeUrlString()).Result;
        //if (imdbPerson != null)
        //    await SaveImdbWorks(benefit, imdbPerson);
        var evt = _Mapper.Map<BeneficiaryRegistered, BeneficiaryImdbIntegrated>(message);
        await _Publisher.Publish(evt);
        await _BenefitRepository.SaveChangesAsync();
        return Sucess();
    }

    private async Task<bool> SaveImdbWorks(Beneficiary benefit, ImdbResponse imdbPerson)
    {
        if (imdbPerson.errorMessage.IsFilled())
            throw new DomainRuleException(imdbPerson.errorMessage);
        if (imdbPerson.results == null || imdbPerson.results.Count == 0)
        {
            _Logger.LogWarning($"No work found on Imdb Api for the name \"{benefit.Name}\".");
            return false;
        }
        var works = imdbPerson.results.Select(o => new ImdbWork(o.title, o.image, o.description, benefit.ID)).ToArray();
        await _BenefitRepository.AddImdbWorksAsync(works, false);
        return true;
    }
}