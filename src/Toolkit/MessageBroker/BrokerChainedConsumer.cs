using MassTransit;
using Toolkit.Interfaces;
using Toolkit.Mapper;

namespace Toolkit.MessageBroker;

public abstract class BrokerChainedConsumer<T, V> : BrokerConsumer<T>
    where T : class
    where V : class
{

    protected override void OnConsumed(ConsumeContext<T> context, BrokerConsumerResult previousResult)
    {
        base.OnConsumed(context, previousResult);
        if (previousResult.ResultType != BrokerConsumerResultType.Sucess)
            return;
        var result = (BrokerConsumerSucess)previousResult;
        if (result.GeneratedArtifact == null)
            return;
        var mapper = MapperFactory.Map<IIdentifiable, V>();
        V chainedMessage = mapper.Map<IIdentifiable, V>(result.GeneratedArtifact);
        context.Publish(chainedMessage);
    }
}