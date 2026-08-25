using Skycoin.Alerting;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 16 * 1024);

var app = builder.Build();
var store = new AlertStore(capacity: 1000);
var evaluatedTotal = 0L;
var rejectedTotal = 0L;

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Cache-Control"] = "no-store";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});

IResult Evaluate(AlertRequest request)
{
    try
    {
        var evaluation = AlertEngine.Evaluate(request.Metric ?? string.Empty, request.Value, request.Threshold);
        Interlocked.Increment(ref evaluatedTotal);

        StoredAlert? persisted = null;
        if (evaluation.Severity is AlertSeverity.Warning or AlertSeverity.Critical)
        {
            persisted = store.Add(evaluation, DateTimeOffset.UtcNow);
        }

        app.Logger.LogInformation(
            "evaluated metric {Metric} with severity {Severity}",
            evaluation.Metric,
            evaluation.Severity);

        return Results.Ok(new
        {
            metric = evaluation.Metric,
            value = evaluation.Value,
            threshold = evaluation.Threshold,
            severity = evaluation.Severity.ToString().ToLowerInvariant(),
            alert = persisted
        });
    }
    catch (ArgumentException exception)
    {
        Interlocked.Increment(ref rejectedTotal);
        return Results.BadRequest(new { error = exception.Message });
    }
}

app.MapPost("/api/v1/evaluate", Evaluate);
app.MapPost("/api/v1/alert", Evaluate); // compatibility route
app.MapGet("/api/v1/alerts", () => Results.Ok(store.Snapshot()));
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "sky-alert-engine" }));
app.MapGet("/ready", () => Results.Ok(new { status = "ready", capacity = store.Capacity }));
app.MapGet("/metrics", () => Results.Ok(new
{
    evaluated_total = Interlocked.Read(ref evaluatedTotal),
    rejected_total = Interlocked.Read(ref rejectedTotal),
    alerts_retained = store.Count,
    alerts_evicted = store.Evicted,
    alert_capacity = store.Capacity
}));

app.Run("http://0.0.0.0:8080");

public sealed record AlertRequest(string? Metric, double Value, double Threshold);
