using MassTransit;
using Toolkit.Interfaces;
using System.Diagnostics;
using MassTransit.Metadata;

namespace Toolkit.MessageBroker;

public abstract class BrokerConsumer<T> : IBrokerConsumer<T> where T : class
{
    protected abstract Task<BrokerConsumerResult> ConsumeAsync(T message);

    protected virtual void OnConsumed(ConsumeContext<T> context, BrokerConsumerResult previousResult)
    {
    }

    protected BrokerConsumerResult Sucess(IIdentifiable generatedArtifact = null)
    {
        return new BrokerConsumerSucess(generatedArtifact);
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        var timer = Stopwatch.StartNew();
        string consumerName = GetType().Name;
        if (context == null || context.Message == null)
            return;
        var result = await ConsumeAsync(context.Message);
        if (result.ResultType != BrokerConsumerResultType.Sucess)
            return;
        OnConsumed(context, result);
    }
}