using Toolkit.MessageBroker;
using Benefit.Domain.Events;
using Toolkit.Configurations;
using Benefit.Domain.Operator;
using Benefit.Domain.Interfaces;

namespace Benefit.Service.Workers;

public sealed class BenefitInsertedConsumer : BrokerChainedConsumer<BenefitInsertedEvent, BenefitCreatedEvent>
{
    public BenefitInsertedConsumer(IBenefitRepository benefitRepository, GenericMapper mapper)
        : base(mapper)
    {
        _BenefitRepository = benefitRepository;
    }

    private readonly IBenefitRepository _BenefitRepository;

    protected override BrokerConsumerResult Consume(BenefitInsertedEvent eventData)
    {
        var op = Operator.CreateOperator(eventData.Operator);
        var beneficiary = op.CreateBeneficiary(eventData.Name, eventData.CPF, eventData.BirthDate);
        _BenefitRepository.Add(beneficiary);
        return BrokerConsumerResult.Sucess;
    }
}