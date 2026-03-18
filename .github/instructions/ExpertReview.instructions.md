---
applyTo:
  - "src/Compiler/**/*.{fs,fsi}"
---

# Compiler Review Rules

## Test discipline

- Add tests for every behavioral change before merging — missing coverage is the most common review blocker.
- Explain all new errors in test baselines and confirm each is expected behavior, not a regression.
- Test on IL-defined members, not just F#-defined ones. Check `UseNullAsTrueValue` edge cases.

## Type safety

- Use `tryTcrefOfAppTy` or the `AppTy` active pattern instead of direct `TType` pattern matches — direct matches miss erased types.
- Always call `stripTyEqns` before pattern matching on types. Never match `TType_app` directly.
- Remove default wildcard patterns in discriminated union matches so the compiler catches missing cases.
- Raise internal compiler errors for unexpected type forms (`TType_ucase`) rather than returning defaults.

## Feature gating

- Gate every new language feature behind a `LanguageFeature` flag and ship off-by-default until stable.
- Factor cleanup changes into separate commits from feature enablement.
- Major language changes require an RFC before implementation.

## Code generation

- Strip debug points when matching on `Expr.Lambda` during code generation to prevent IL stack corruption.
- Verify no tail-call behavior changes — check IL diffs before and after.
- After stripping a lambda for a delegate, bind discarded unit parameters using `BindUnitVars`.

## Backward compatibility

- Codegen changes that depend on new FSharp.Core functions must guard against older FSharp.Core versions.
- Do not alter the C#/.NET visible assembly surface without treating it as a breaking change.

## Concurrency

- Thread cancellation tokens through all async operations; uncancellable long-running operations are blocking bugs.
- Do not add catch-all exception handlers. Never swallow `OperationCanceledException`.

## Performance

- Performance claims require `--times` output, benchmarks, or profiler evidence. Do not inline large functions without data.
