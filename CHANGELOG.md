# Changelog

## Unreleased

### Added

- deterministic `AlertEngine` severity evaluation
- bounded concurrent `AlertStore` with eviction counters
- `/api/v1/evaluate`, readiness, and process-local metrics endpoints
- separate xUnit test project for engine/store behavior
- non-root multi-stage container packaging
- Release build, tests, dependency audit, container and runtime smoke CI gates
- security and integration documentation

### Changed

- preserved `/api/v1/alert` as a compatibility route while moving business rules out of `Program.cs`
- clarified the repository as an engineering-beta alert-evaluation primitive rather than a complete monitoring platform
