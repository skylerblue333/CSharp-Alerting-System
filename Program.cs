using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var alerts = new ConcurrentQueue<Alert>();
const int maxAlerts = 1000;

app.MapPost("/api/v1/alert", (AlertRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Metric) || request.Metric.Length > 200)
        return Results.BadRequest(new { error = "metric must contain 1-200 characters" });
    if (double.IsNaN(request.Value) || double.IsInfinity(request.Value) ||
        double.IsNaN(request.Threshold) || double.IsInfinity(request.Threshold))
        return Results.BadRequest(new { error = "value and threshold must be finite numbers" });

    var severity = request.Value > request.Threshold * 1.5
        ? "critical"
        : request.Value > request.Threshold
            ? "warning"
            : "ok";

    var alert = new Alert(
        request.Metric.Trim(), request.Value, request.Threshold, severity, DateTimeOffset.UtcNow);

    if (severity != "ok")
    {
        alerts.Enqueue(alert);
        while (alerts.Count > maxAlerts && alerts.TryDequeue(out _)) { }
    }

    return Results.Ok(alert);
});

app.MapGet("/api/v1/alerts", () => Results.Ok(alerts.ToArray()));
app.MapGet("/health", () => Results.Ok(new { status = "healthy", version = "3.1.0" }));

app.Run("http://0.0.0.0:8080");

record AlertRequest(string? Metric, double Value, double Threshold);
record Alert(string Metric, double Value, double Threshold, string Severity, DateTimeOffset Timestamp);
