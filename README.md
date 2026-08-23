# CSharp-Alerting-System

A small ASP.NET Core 8 alert-evaluation service for the SKYCOIN4444 ecosystem.

## Current implementation

- `POST /api/v1/alert` evaluates a metric against a threshold.
- Severity is `ok`, `warning`, or `critical`.
- Non-OK alerts are retained in a bounded in-memory queue (maximum 1,000 alerts).
- `GET /api/v1/alerts` returns retained alerts.
- `GET /health` reports service health.
- Request validation rejects empty/overlong metric names and non-finite numeric values.

## Example

```bash
curl -X POST http://localhost:8080/api/v1/alert \
  -H 'Content-Type: application/json' \
  -d '{"metric":"cpu","value":92,"threshold":80}'
```

## Setup

Requires .NET 8 SDK.

```bash
dotnet restore
dotnet run
```

The service listens on port `8080`.

## Testing

A production-quality test suite is not currently present. Do not interpret the API implementation or CI configuration as proof of complete test coverage or production readiness.

## Production limitations

This repository currently uses in-memory storage. Alerts are lost when the process restarts. It does not yet provide durable persistence, authentication/authorization, external notification providers, distributed coordination, or production observability.

Those capabilities belong in the wider SKYCOIN4444 alerting/notification architecture and should be added only with appropriate contracts and tests.

## Architecture role

```text
Metrics / Services
        ↓
Alert Evaluation
        ↓
Bounded Alert Store
        ↓
Events / Notification Adapters
```

This repository is a focused alert-evaluation primitive, not a complete enterprise alerting platform.

## License

See the repository `LICENSE` file if present. Third-party dependencies remain subject to their respective licenses.

## Authorship

Developed by **Skyler Blue Spillers**, with assistance from humans, open-source software, automation, and occasionally robot slaves. 🤖

**AI-assisted ≠ solely AI-authored.**

## SKYCOIN4444

- Website: https://skycoin4444.com
- Network: https://skycoin4444.net
- Shop: https://skycoin4444.shop
- Token: https://skycoin44.token
- GitHub: https://github.com/skylerblue333
