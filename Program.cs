using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlertingSystem
{
    public enum Severity { Info, Warning, Critical }

    public record Alert(string Id, string Message, Severity Severity, DateTime Timestamp);

    public class AlertEngine
    {
        private readonly List<Alert> _history = new();

        public async Task<bool> FireAlert(string message, Severity severity)
        {
            var alert = new Alert(
                Id: Guid.NewGuid().ToString("N")[..8],
                Message: message,
                Severity: severity,
                Timestamp: DateTime.UtcNow
            );

            _history.Add(alert);
            await RouteAlert(alert);
            return true;
        }

        private async Task RouteAlert(Alert alert)
        {
            await Task.Delay(50); // Simulate async dispatch
            string icon = alert.Severity switch
            {
                Severity.Critical => "🔴",
                Severity.Warning  => "🟡",
                _                 => "🔵"
            };
            Console.WriteLine($"[{alert.Severity}] {icon} [{alert.Id}] {alert.Message} @ {alert.Timestamp:HH:mm:ss}");
        }
    }

    class Program
    {
        static async Task Main()
        {
            var engine = new AlertEngine();
            Console.WriteLine("=== Alerting System Online ===\n");
            await engine.FireAlert("CPU usage at 95%", Severity.Critical);
            await engine.FireAlert("Memory usage at 78%", Severity.Warning);
            await engine.FireAlert("Health check passed", Severity.Info);
        }
    }
}
