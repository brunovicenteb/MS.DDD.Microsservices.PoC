namespace Toolkit.TransactionalOutBox;

public interface IOpenTelemetreable
{
    public IDatabaseable UseOpenTelemetry();

    public IDatabaseable DoNotOpenTelemetry();
}