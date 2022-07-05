using Toolkit.MessageBroker;

namespace Benefit.Consumer.Worker.Workers;

public sealed class BenefitConsumerFactory : BrokerConsumerFactory
{
    protected override void DeclareConsumers()
    {
        AddConsumer<BenefitInsertedConsumer>();
    }
}