---
applyTo:
  - "src/Compiler/**/*.{fs,fsi}"
---

# Compiler Review Rules

## Test discipline

- Add tests for every behavioral change before merging — missing coverage is the most common review blocker.
- Explain all new errors in test baselines and confirm each is expected behavior, not a regression.

## Type safety

- Use `tryTcrefOfAppTy` or the `AppTy` active pattern instead of direct `TType` pattern matches — direct matches miss erased types.
- Remove default wildcard patterns in discriminated union matches so the compiler catches missing cases.

## Feature gating

- Gate every new language feature behind a `LanguageFeature` flag and ship off-by-default until stable.
- Factor cleanup changes into separate commits from feature enablement.

## Code generation

- Strip debug points when matching on `Expr.Lambda` during code generation to prevent IL stack corruption.
