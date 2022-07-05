using MassTransit;
using System.Diagnostics;
using MassTransit.Metadata;
using Benefit.Domain.Events;
using Benefit.Domain.Benefit;
using Benefit.Domain.Operator;
using System.Collections.Concurrent;

namespace Benefit.Consumer.Worker.Workers;

public sealed class BenefitInsertedConsumer : IConsumer<BenefitInsertedEvent>
{
    private static readonly ConcurrentDictionary<uint, Beneficiary> _Cache = new ConcurrentDictionary<uint, Beneficiary>();

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
            BaseOperator op = OperatorFactory.CreateOperator(evt.Operator);
            Beneficiary beneficiary = op.CreateBeneficiary(evt.ParentID, evt.Name, evt.CPF, evt.BirthDate);
            _Cache.TryAdd(beneficiary.ID, beneficiary);
        }
        catch (Exception ex)
        {
            await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<BenefitInsertedEvent>.ShortName, ex);
        }
    }
}