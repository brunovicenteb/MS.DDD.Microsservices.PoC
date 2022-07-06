using MassTransit;
using System.Diagnostics;
using MassTransit.Metadata;
using Benefit.Domain.Events;
using Benefit.Domain.Interfaces;
using Benefit.Domain.Operator;

namespace Benefit.Service.Workers;

public sealed class BenefitInsertedConsumer : IConsumer<BenefitInsertedEvent>
{
    public BenefitInsertedConsumer(IBenefitRepository benefitRepository)
    {
        _BenefitRepository = benefitRepository;
    }

    private readonly IBenefitRepository _BenefitRepository;

    public async Task Consume(ConsumeContext<BenefitInsertedEvent> context)
    {
        var timer = Stopwatch.StartNew();
        try
        {
            if (context == null || context.Message == null)
            {
                await Console.Out.WriteAsync("BenefitInsertedEvent Called with no Message.");
                return;
            }
            await Console.Out.WriteAsync("BenefitInsertedEvent Called");
            var evt = context.Message;
            var op = Operator.CreateOperator(evt.Operator);
            var beneficiary = op.CreateBeneficiary(evt.Name, evt.CPF, evt.BirthDate);
            _BenefitRepository.Add(beneficiary);
        }
        catch (Exception ex)
        {
            await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<BenefitInsertedEvent>.ShortName, ex);
        }
    }
}