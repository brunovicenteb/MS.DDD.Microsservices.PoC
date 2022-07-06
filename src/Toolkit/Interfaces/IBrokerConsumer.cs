using MassTransit;

public interface IBrokerConsumer : IConsumer
{
}

public interface IBrokerConsumer<in T> : IConsumer<T>, IBrokerConsumer where T : class
{
}