# Sky Alert Engine

A focused ASP.NET Core 8 metric-threshold evaluation service for the SKYCOIN4444 engineering portfolio.

**Status: engineering beta.** The implementation is suitable for local development and integration testing after CI passes. Production deployment is not claimed.

## What it does

- Deterministically evaluates a metric value against a non-negative threshold.
- Returns `ok`, `warning`, or `critical` severity.
- Retains only warning/critical alerts in a bounded in-memory store.
- Exposes health, readiness, and process-local metrics endpoints.
- Applies request-size limits and basic response security headers.
- Runs as a non-root container.

## API

### Evaluate a metric

```bash
curl -X POST http://localhost:8080/api/v1/evaluate \
  -H 'Content-Type: application/json' \
  -d '{"metric":"cpu.utilization","value":92,"threshold":80}'
```

`POST /api/v1/alert` remains as a compatibility alias.

Severity rules:

- `ok`: value is less than or equal to the threshold
- `warning`: value is above the threshold and at most 1.5x the threshold
- `critical`: value is above 1.5x the threshold

### Operational endpoints

- `GET /api/v1/alerts` — current retained warning/critical alerts
- `GET /health` — liveness
- `GET /ready` — readiness and configured alert capacity
- `GET /metrics` — process-local evaluation/rejection/retention counters

## Development

Requires the .NET 8 SDK.

```bash
dotnet restore CSharp-Alerting-System.csproj
dotnet restore tests/Alerting.Tests.csproj
dotnet build CSharp-Alerting-System.csproj -c Release
dotnet test tests/Alerting.Tests.csproj -c Release
dotnet run --project CSharp-Alerting-System.csproj
```

## Container

```bash
docker build -t sky-alert-engine .
docker run --rm -p 8080:8080 sky-alert-engine
```

The runtime image uses an unprivileged user.

## Architecture

```text
metric sample
    |
    v
AlertEngine (pure deterministic evaluation)
    |
    +---- ok ----------------------> response only
    |
    +---- warning / critical -----> bounded AlertStore
                                      |
                                      +--> alerts API / metrics
```

The engine and bounded store are independently testable. HTTP routes are deliberately thin adapters over those primitives.

## SKYCOIN4444 integration

A future ecosystem adapter can feed trusted metric samples into this service and consume alert events through a stable interface. The standalone repository remains independently deployable; it should not be copied wholesale into the flagship application.

## Explicit limitations

This is not a complete monitoring or incident-management platform. It does **not** currently provide durable persistence, authentication/RBAC, external notification delivery, deduplication windows, silencing, escalation policies, distributed coordination, multi-tenant isolation, HA, TLS termination, or a verified production deployment.

Alerts are process-local and are lost on restart. The exposed metrics are JSON counters rather than a Prometheus/OpenTelemetry compatibility claim.

See `SECURITY.md` for security boundaries and `CHANGELOG.md` for productization history.

## License

See `LICENSE`.
