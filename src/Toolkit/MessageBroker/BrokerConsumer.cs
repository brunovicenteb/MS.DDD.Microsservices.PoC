using MassTransit;
using System.Diagnostics;
using MassTransit.Metadata;
using Toolkit.Interfaces;

namespace Toolkit.MessageBroker;

public abstract class BrokerConsumer<T> : IBrokerConsumer<T> where T : class
{
    protected abstract BrokerConsumerResult Consume(T message);

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
        try
        {
            string consumerName = GetType().Name;
            if (context == null || context.Message == null)
            {
                await Console.Out.WriteAsync($"{consumerName} called without message.");
                return;
            }
            var result = Consume(context.Message);
            if (result.ResultType != BrokerConsumerResultType.Sucess)
                return;
            await Console.Out.WriteAsync($"{consumerName} successfully completed.");
            await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<T>.ShortName);
            OnConsumed(context, result);
        }
        catch (Exception ex)
        {
            await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<T>.ShortName, ex);
        }
    }
}