using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var alerts = new ConcurrentQueue<object>();

app.MapPost("/api/v1/alert", async (HttpContext context) => {
    using var doc = await JsonDocument.ParseAsync(context.Request.Body);
    var root = doc.RootElement;
    
    var metric = root.TryGetProperty("metric", out var m) ? m.GetString() : "unknown";
    var value = root.TryGetProperty("value", out var v) ? v.GetDouble() : 0;
    var threshold = root.TryGetProperty("threshold", out var t) ? t.GetDouble() : 100;
    
    var severity = value > threshold * 1.5 ? "critical" : value > threshold ? "warning" : "ok";
    var alert = new { metric, value, threshold, severity, timestamp = DateTimeOffset.UtcNow };
    
    if (severity != "ok") alerts.Enqueue(alert);
    
    await context.Response.WriteAsJsonAsync(alert);
});

app.MapGet("/api/v1/alerts", () => alerts.ToArray());
app.MapGet("/health", () => new { status = "healthy", version = "3.0.0" });

app.Run("http://0.0.0.0:8080");
