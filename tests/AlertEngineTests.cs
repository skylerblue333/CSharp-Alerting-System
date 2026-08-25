using Skycoin.Alerting;

namespace Skycoin.Alerting.Tests;

public sealed class AlertEngineTests
{
    [Theory]
    [InlineData(50, 100, AlertSeverity.Ok)]
    [InlineData(100, 100, AlertSeverity.Ok)]
    [InlineData(101, 100, AlertSeverity.Warning)]
    [InlineData(150, 100, AlertSeverity.Warning)]
    [InlineData(151, 100, AlertSeverity.Critical)]
    public void EvaluatesSeverityDeterministically(double value, double threshold, AlertSeverity expected)
    {
        var result = AlertEngine.Evaluate("cpu.utilization", value, threshold);
        Assert.Equal(expected, result.Severity);
    }

    [Fact]
    public void TrimsMetricName()
    {
        var result = AlertEngine.Evaluate("  cpu  ", 1, 2);
        Assert.Equal("cpu", result.Metric);
    }

    [Fact]
    public void RejectsInvalidMetric()
    {
        Assert.Throws<ArgumentException>(() => AlertEngine.Evaluate(" ", 1, 1));
    }

    [Fact]
    public void RejectsNegativeThreshold()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => AlertEngine.Evaluate("cpu", 1, -1));
    }

    [Fact]
    public void RejectsNonFiniteNumbers()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => AlertEngine.Evaluate("cpu", double.NaN, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => AlertEngine.Evaluate("cpu", 1, double.PositiveInfinity));
    }
}
