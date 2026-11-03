---
applyTo: 'src/Compiler/Facilities/**'
---

Compiler infrastructure helpers for diagnostics and reference resolution: DiagnosticsLogger, ReferenceResolver, BuildGraph.

- Preserve the null fast-path in `GetOrComputeValue` so already-materialized nodes avoid taking the semaphore or allocating a new async workflow.
- Use `LegacyReferenceResolver` only through `ILegacyReferenceResolver.Resolve`, and preserve the `LegacyResolutionEnvironment` distinction between editing and execution.
- Always wrap memoized jobs in `Async.TryCancelled` and `Async.Catch` and return the captured logger, or cancellations and failures bypass cache bookkeeping.
- Route compiler failures through `DiagnosticsLogger` exception types like `WrappedError`, `ReportedError`, and `StopProcessingExn` instead of throwing ad hoc exceptions.
- Keep `AttachRange` mapping unresolved-reference exceptions to range-bearing forms before they reach reporting code.
