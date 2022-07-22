namespace Toolkit.OutBox;

public interface ITelemetreable
{
    public IDatabaseable UseTelemetry(string telemetryHost = "TELEMETRY_HOST");

    public IDatabaseable DoNotUseTelemetry();
}