namespace Toolkit.OutBox;

public interface IBrokeable
{
    public void UseRabbitMq(string rabbitMqVariableName = "RABBIT_MQ");
}