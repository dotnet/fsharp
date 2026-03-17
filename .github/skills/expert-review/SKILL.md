---
name: reviewing-compiler-prs
description: "Performs multi-dimensional expert review of F# compiler PRs across 15 dimensions: type checking, IL emission, binary compatibility, AST correctness, parallel determinism, concurrency safety, IDE performance, diagnostics, feature gating, and code quality. Invoke when reviewing compiler changes, requesting pre-merge feedback, or checking PR quality against established review standards."
---

# Expert Review

Full review logic lives in the `expert-reviewer` agent. This skill provides dimension selection and a self-review checklist.

For generic multi-model review (not F#-specific), see the `review-council` skill instead.

## When to Invoke

- PR touches `src/Compiler/` — invoke the `expert-reviewer` agent
- PR touches `src/FSharp.Core/` — focus on API Surface & Concurrency dimensions
- PR touches `vsintegration/` or `LanguageServer/` — focus on IDE Performance & Editor UX
- PR touches `tests/` only — quick check: baselines explained? Cross-TFM coverage?
- Pre-merge self-review — use the checklist below

## Dimension Selection

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

## Self-Review Checklist

1. [ ] Every behavioral change has a test
2. [ ] Test baselines updated with explanations for new errors
3. [ ] New language features have a `LanguageFeature` guard
4. [ ] No unintended public API surface changes
5. [ ] Cleanup changes separate from feature enablement
6. [ ] Compiler warnings resolved
7. [ ] Tests pass in both Debug and Release
8. [ ] Error messages follow: statement → analysis → advice
