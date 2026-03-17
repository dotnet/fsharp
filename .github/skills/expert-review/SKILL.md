---
name: expert-review
description: "Performs multi-dimensional expert review of F# compiler PRs. Evaluates type checking correctness, IL emission, binary compatibility, AST accuracy, IDE performance, test coverage, and code quality. Invoke when reviewing code changes, checking PR quality, or requesting thorough feedback on compiler modifications."
---

# Expert Review

Performs a thorough, multi-dimensional review of F# compiler changes. The full review logic lives in `.github/agents/expert-reviewer.md` — invoke that agent for the actual review.

## When to Invoke

- PR touches `src/Compiler/` — invoke the full agent
- PR touches `src/FSharp.Core/` — invoke with focus on API Surface & Concurrency
- PR touches `vsintegration/` or LanguageServer — invoke with focus on IDE Performance & Editor UX
- PR touches `tests/` only — quick check: are baselines explained? Cross-TFM coverage present?
- Pre-merge self-review — use the quick checklist below

## Dimension Selection

Not every PR needs all 15 dimensions. Select based on changed files:

| Files Changed | Focus Dimensions |
|---|---|
| `Checking/`, `TypedTree/` | Type System, Parallel Compilation, Feature Gating |
| `CodeGen/`, `AbstractIL/`, `Optimize/` | IL Emission, Debug Correctness, Test Coverage |
| `SyntaxTree/`, `pars.fsy` | Parser Integrity, Feature Gating |
| `TypedTreePickle.*`, `CompilerImports.*` | Binary Compatibility (highest priority) |
| `Service/`, `LanguageServer/` | IDE Performance, Concurrency |
| `FSComp.txt` | Diagnostic Quality |
| `FSharp.Core/` | API Surface, Concurrency |
| `vsintegration/` | Editor Integration |

## Quick Self-Review Checklist

Before requesting a full agent review, verify:

1. [ ] Every behavioral change has a test
2. [ ] Test baselines are updated with explanations for new errors
3. [ ] New language features have a `LanguageFeature` guard
4. [ ] No unintended public API surface changes
5. [ ] Cleanup changes are separate from feature enablement
6. [ ] Compiler warnings are resolved
7. [ ] Tests run in both Debug and Release where relevant
8. [ ] Error messages follow: statement → analysis → advice

## Invocation

```
Invoke the expert-reviewer agent to review this PR.
```

The agent executes a 5-wave review workflow (Orientation → Structural → Correctness → Integration → Quality) and reports findings by severity.
