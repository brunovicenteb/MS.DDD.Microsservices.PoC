using MassTransit;
using System.Diagnostics;
using MassTransit.Metadata;

namespace Toolkit.MessageBroker;

public enum BrokerConsumerResult
{
    Sucess
}

public abstract class BrokerConsumer<T> : IBrokerConsumer<T> where T : class
{
    protected abstract BrokerConsumerResult Consume(T message);

    protected virtual void OnSuccessfullyConsumed(ConsumeContext<T> context)
    {
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
            if (result != BrokerConsumerResult.Sucess)
                return;
            await Console.Out.WriteAsync($"{consumerName} successfully completed.");
            await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<T>.ShortName);
            OnSuccessfullyConsumed(context);
        }
        catch (Exception ex)
        {
            await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<T>.ShortName, ex);
        }
    }
}