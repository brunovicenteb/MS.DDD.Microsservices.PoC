using MassTransit;
using Toolkit.Configurations;

namespace Toolkit.MessageBroker;

public abstract class BrokerChainedConsumer<T, V> : BrokerConsumer<T>
    where T : class
    where V : class
{
    public BrokerChainedConsumer(GenericMapper mapper)
    {
        _Mapper = mapper;
    }

    private readonly GenericMapper _Mapper;

    protected override sealed void OnSuccessfullyConsumed(ConsumeContext<T> context)
    {
        base.OnSuccessfullyConsumed(context);
        V chainedMessage = _Mapper.Map<T, V>(context.Message);
        context.Publish<V>(chainedMessage);
    }
}