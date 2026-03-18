---
applyTo:
  - "src/Compiler/**/*.{fs,fsi}"
---

# Compiler Review Rules

## Type safety

- Always call `stripTyEqns` before pattern matching on types. Use `AppTy` active pattern, not `TType_app` directly.
- Remove default wildcard patterns in discriminated union matches so the compiler catches missing cases.
- Raise internal compiler errors for unexpected type forms rather than returning defaults.

## Feature gating

- Gate every new language feature behind a `LanguageFeature` flag and ship off-by-default until stable.
- Factor cleanup changes into separate commits from feature enablement.
- Major language changes require an RFC before implementation.

## Binary compatibility

- Codegen changes that depend on new FSharp.Core functions must guard against older FSharp.Core versions.
- Do not alter the C#/.NET visible assembly surface without treating it as a breaking change.

## Concurrency

- Thread cancellation tokens through all async operations; uncancellable long-running operations are blocking bugs.
- Do not add catch-all exception handlers. Never swallow `OperationCanceledException`.

## Performance

- Performance claims require `--times` output, benchmarks, or profiler evidence.
