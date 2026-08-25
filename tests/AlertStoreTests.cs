using Skycoin.Alerting;
using Xunit;

namespace Skycoin.Alerting.Tests;

public sealed class AlertStoreTests
{
    [Fact]
    public void EvictsOldestAlertsAtCapacity()
    {
        var store = new AlertStore(capacity: 2);
        var first = AlertEngine.Evaluate("cpu", 11, 10);
        var second = AlertEngine.Evaluate("memory", 11, 10);
        var third = AlertEngine.Evaluate("disk", 11, 10);

        store.Add(first, DateTimeOffset.UnixEpoch);
        store.Add(second, DateTimeOffset.UnixEpoch.AddSeconds(1));
        store.Add(third, DateTimeOffset.UnixEpoch.AddSeconds(2));

        var alerts = store.Snapshot();
        Assert.Equal(2, alerts.Length);
        Assert.Equal("memory", alerts[0].Metric);
        Assert.Equal("disk", alerts[1].Metric);
        Assert.Equal(1, store.Evicted);
    }

    [Fact]
    public void RejectsInvalidCapacity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AlertStore(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new AlertStore(100_001));
    }
}
