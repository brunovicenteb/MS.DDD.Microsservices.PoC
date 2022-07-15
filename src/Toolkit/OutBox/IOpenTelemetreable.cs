namespace Toolkit.OutBox;

public interface IOpenTelemetreable
{
    public IDatabaseable UseOpenTelemetry();

    public IDatabaseable DoNotOpenTelemetry();
}