using MassTransit;
using Microsoft.Extensions.Logging;

namespace Toolkit.MessageBroker;

public abstract class BrokerConsumer
{
    private static int _RetryCount = 5;
    private static int _RetryIntevalInMilliseconds = 100;

    public static void SetRetryParameters(int retryCount, int retryIntevalInMilliseconds)
    {
        _RetryCount = retryCount;
        _RetryIntevalInMilliseconds = retryIntevalInMilliseconds;
    }

    protected virtual int RetryCount
        => _RetryCount;

    protected virtual int RetryIntevalInMilliseconds
        => _RetryIntevalInMilliseconds;
}

public abstract class BrokerConsumer<T> : BrokerConsumer, IBrokerConsumer<T> where T : class
{
    protected abstract Task ConsumeAsync(T message);

    protected abstract ILogger Logger { get; }

    public async Task Consume(ConsumeContext<T> context)
    {
        if (context == null || context.Message == null)
            return;
        await ConsumeAsync(context.Message);
    }

    protected async Task<TResult> TryExecute<TResult>(Func<Task<TResult>> action, string failMessage = null)
        where TResult : class
    {
        return await Resilience.TryExecute(action, Logger, failMessage, RetryCount, RetryIntevalInMilliseconds);
    }
}