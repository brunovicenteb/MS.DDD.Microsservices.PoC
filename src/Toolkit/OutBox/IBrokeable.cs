namespace Toolkit.OutBox;

public interface IBrokeable
{
    public IAuthenticable UseRabbitMq(string rabbitMqVariableName = "RABBIT_MQ");
    public IAuthenticable UseHarness();
}