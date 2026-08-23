using Xunit;
using Skycoin.Alerting;

namespace Skycoin.Alerting.Tests;

public class AlertEngineTests
{
    [Fact]
    public void BelowThresholdIsOk()
    {
        var result = AlertEngine.Evaluate("cpu", 50, 100);
        Assert.Equal(AlertSeverity.Ok, result.Severity);
    }

    [Fact]
    public void AboveThresholdIsWarning()
    {
        var result = AlertEngine.Evaluate("cpu", 110, 100);
        Assert.Equal(AlertSeverity.Warning, result.Severity);
    }

    [Fact]
    public void FarAboveThresholdIsCritical()
    {
        var result = AlertEngine.Evaluate("cpu", 151, 100);
        Assert.Equal(AlertSeverity.Critical, result.Severity);
    }

    [Fact]
    public void InvalidMetricIsRejected()
    {
        Assert.Throws<ArgumentException>(() => AlertEngine.Evaluate(" ", 1, 1));
    }
}
