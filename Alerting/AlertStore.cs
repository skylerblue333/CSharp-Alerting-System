using System.Collections.Concurrent;

namespace Skycoin.Alerting;

public sealed record StoredAlert(
    long Id,
    string Metric,
    double Value,
    double Threshold,
    AlertSeverity Severity,
    DateTimeOffset Timestamp);

public sealed class AlertStore
{
    private readonly ConcurrentQueue<StoredAlert> _alerts = new();
    private readonly int _capacity;
    private long _nextId;
    private long _evicted;

    public AlertStore(int capacity = 1000)
    {
        if (capacity is < 1 or > 100_000)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _capacity = capacity;
    }

    public StoredAlert Add(AlertEvaluation evaluation, DateTimeOffset timestamp)
    {
        ArgumentNullException.ThrowIfNull(evaluation);
        var alert = new StoredAlert(
            Interlocked.Increment(ref _nextId),
            evaluation.Metric,
            evaluation.Value,
            evaluation.Threshold,
            evaluation.Severity,
            timestamp);

        _alerts.Enqueue(alert);
        while (_alerts.Count > _capacity && _alerts.TryDequeue(out _))
        {
            Interlocked.Increment(ref _evicted);
        }

        return alert;
    }

    public StoredAlert[] Snapshot() => _alerts.ToArray();

    public int Count => _alerts.Count;

    public long Evicted => Interlocked.Read(ref _evicted);

    public int Capacity => _capacity;
}
