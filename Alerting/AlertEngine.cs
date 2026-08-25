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
    public const int MaxMetricLength = 128;

    public static AlertEvaluation Evaluate(string metric, double value, double threshold)
    {
        if (string.IsNullOrWhiteSpace(metric))
        {
            throw new ArgumentException("Metric is required.", nameof(metric));
        }

        var normalizedMetric = metric.Trim();
        if (normalizedMetric.Length > MaxMetricLength)
        {
            throw new ArgumentException($"Metric must be at most {MaxMetricLength} characters.", nameof(metric));
        }

        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be finite.");
        }

        if (!double.IsFinite(threshold) || threshold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold must be finite and non-negative.");
        }

        var severity = value > threshold * 1.5
            ? AlertSeverity.Critical
            : value > threshold
                ? AlertSeverity.Warning
                : AlertSeverity.Ok;

        return new AlertEvaluation(normalizedMetric, value, threshold, severity);
    }
}
