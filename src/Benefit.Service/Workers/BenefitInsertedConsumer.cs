using Toolkit.MessageBroker;
using Benefit.Domain.Events;
using Benefit.Domain.Operator;
using Benefit.Domain.Interfaces;

namespace Benefit.Service.Workers;

public sealed class BenefitInsertedConsumer : BrokerChainedConsumer<BenefitInsertedEvent, BenefitCreatedEvent>
{
    public BenefitInsertedConsumer(IBenefitRepository benefitRepository)
    {
        _BenefitRepository = benefitRepository;
    }

    private readonly IBenefitRepository _BenefitRepository;

    protected override BrokerConsumerResult Consume(BenefitInsertedEvent eventData)
    {
        var op = Operator.CreateOperator(eventData.Operator);
        var beneficiary = op.CreateBeneficiary(eventData.Name, eventData.CPF, eventData.BirthDate);
        beneficiary = _BenefitRepository.Add(beneficiary);
        return Sucess(beneficiary);
    }
}