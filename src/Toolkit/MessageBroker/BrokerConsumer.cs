using MassTransit;
using Toolkit.Interfaces;
using MassTransit.RetryPolicies;
using Microsoft.Extensions.Logging;

namespace Toolkit.MessageBroker;

public abstract class BrokerConsumer<T> : IBrokerConsumer<T> where T : class
{
    protected abstract Task ConsumeAsync(T message);

    protected abstract ILogger Logger { get; }

    public async Task Consume(ConsumeContext<T> context)
    {
        if (context == null || context.Message == null)
            return;
        await ConsumeAsync(context.Message);
    }

    protected async Task<TResult> TryExecute<TResult>(Func<Task<TResult>> action, string failMessage = null, int retryCount = 5, double intevalInMilliseconds = 100)
        where TResult : class
    {
        try
        {
            int count = 0;
            var policy = Retry.Interval(retryCount, TimeSpan.FromMilliseconds(intevalInMilliseconds));
            var result = await policy.Retry(async () =>
            {
                if (count > 0)
                    Logger.LogInformation($"Attemp #{++count}");
                return await action();
            });
            return result;
        }
        catch (Exception ex)
        {
            if (failMessage.IsFilled())
                Logger.LogError(failMessage, ex);
        }
        return default(TResult);
    }
}