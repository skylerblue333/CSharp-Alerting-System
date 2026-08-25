# Security

## Status

Sky Alert Engine is an **engineering-beta** component. No independent security review or production deployment is claimed.

## Current controls

- metric names are required and bounded
- values and thresholds must be finite; thresholds must be non-negative
- request bodies are limited by Kestrel
- alert retention is bounded to prevent unbounded process-memory growth
- response headers disable MIME sniffing, caching, and referrer disclosure
- the container runs as a non-root user
- CI builds/tests the Release configuration and checks vulnerable NuGet dependencies

## Boundaries

The service currently has no authentication or authorization. Do not expose it directly to untrusted networks or treat submitted metrics as authenticated telemetry.

Alert state is process-local and unencrypted in memory. The service does not provide durable audit logs, secret storage, tenant isolation, TLS termination, notification-provider credentials, or distributed abuse controls.

## Reporting

Report security issues privately to the repository owner. Avoid including active credentials, private keys, access tokens, or customer data in public issues.
