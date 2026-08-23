namespace Skycoin.Alerting;

public enum AlertSeverity
{
    Ok,
    Warning,
    Critical
}

public sealed record AlertEvaluation(string Metric, double Value, double Threshold, AlertSeverity Severity);

public static class AlertEngine
{
    public static AlertEvaluation Evaluate(string metric, double value, double threshold)
    {
        if (string.IsNullOrWhiteSpace(metric))
            throw new ArgumentException("Metric is required.", nameof(metric));
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentOutOfRangeException(nameof(value));
        if (double.IsNaN(threshold) || double.IsInfinity(threshold) || threshold < 0)
            throw new ArgumentOutOfRangeException(nameof(threshold));

        var severity = value > threshold * 1.5
            ? AlertSeverity.Critical
            : value > threshold
                ? AlertSeverity.Warning
                : AlertSeverity.Ok;

        return new AlertEvaluation(metric.Trim(), value, threshold, severity);
    }
}
