using Toolkit.Interfaces;

namespace Toolkit.MessageBroker;

public enum BrokerConsumerResultType
{
    Sucess
}
public abstract class BrokerConsumerResult
{
    public abstract BrokerConsumerResultType ResultType { get; }
}

internal sealed class BrokerConsumerSucess : BrokerConsumerResult
{
    public BrokerConsumerSucess(IIdentifiable generatedArtifact)
    {
        GeneratedArtifact = generatedArtifact;
    }

    public readonly IIdentifiable GeneratedArtifact;
    public override BrokerConsumerResultType ResultType
        => BrokerConsumerResultType.Sucess;
}
