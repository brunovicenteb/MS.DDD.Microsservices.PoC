namespace Toolkit.OutBox.Interfaces;

public interface IOpenTelemetreable
{
    public IDatabaseable UseOpenTelemetry();

    public IDatabaseable DoNotOpenTelemetry();
}