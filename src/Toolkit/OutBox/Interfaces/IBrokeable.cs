namespace Toolkit.OutBox.Interfaces;

public interface IBrokeable
{
    public void UseRabbitMq(string rabbitMqVariableName = "RABBIT_MQ");
}