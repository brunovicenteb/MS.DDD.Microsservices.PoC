using MassTransit;
using Toolkit.Interfaces;
using Toolkit.Configurations;

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
        var mapper = new GenericMapper();
        var result = (BrokerConsumerSucess)previousResult;
        if (result.GeneratedArtifact == null)
            return;
        V chainedMessage = mapper.Map<IIdentifiable, V>(result.GeneratedArtifact);
        context.Publish(chainedMessage);
    }
}