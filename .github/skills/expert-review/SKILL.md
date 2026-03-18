---
name: reviewing-compiler-prs
description: "Performs multi-agent, multi-model code review of F# compiler PRs across 15 dimensions including type checking, IL emission, binary compatibility, parallel determinism, and IDE performance. Dispatches parallel assessment agents per dimension, consolidates with cross-model agreement scoring, and filters false positives. Invoke when reviewing compiler changes, requesting expert feedback, or performing pre-merge quality checks."
---

# Reviewing Compiler PRs

Full dimension definitions and CHECK rules live in the `expert-reviewer` agent.

## When to Invoke

- PR touches `src/Compiler/` — invoke the `expert-reviewer` agent
- PR touches `src/FSharp.Core/` — focus on API Surface & Concurrency
- PR touches `vsintegration/` or `LanguageServer/` — focus on IDE Performance & Editor UX
- PR touches `tests/` only — quick check: baselines explained? Cross-TFM coverage?

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

## Multi-Model Dispatch

Dispatch one agent per selected dimension. For high-confidence reviews, assess each dimension with multiple models (`claude-opus-4.6`, `gemini-3-pro-preview`, `gpt-5.2-codex`). Minimum viable council = 2 models.

**Claims coverage** — before dimension assessment, cross-reference every claim in the PR description and linked issues against actual code changes. Flag orphan claims (stated but not implemented), orphan changes (code changed but not mentioned), and partial implementations.

**Assessment gates** — apply before flagging:
- Understand execution context before judging (test harness ≠ compiler runtime)
- Classify as regression, improvement, or unclear — only regressions are findings
- Require a concrete failing scenario — no hypotheticals
- "Correct convention" for the context in use → discard, not a finding
- "Unexplained" ≠ "wrong" — missing rationale in a commit message is a doc gap, not a defect

**Consolidation:**
1. Deduplicate findings at same location
2. Filter: wrong context → discard; improvement → downgrade; speculation → LOW
3. Classify: Behavioral (correctness) → Quality (structure) → Nitpick (style)
4. Rank by cross-model agreement (≥2 models agree = higher confidence)
5. Present Behavioral first; Nitpicks only if nothing higher — agents love producing nitpicks to have *something* to say, deprioritize them

## Self-Review Checklist

1. [ ] Every behavioral change has a test
2. [ ] Test baselines updated with explanations
3. [ ] New language features have a `LanguageFeature` guard
4. [ ] No unintended public API surface changes
5. [ ] Cleanup changes separate from feature enablement
6. [ ] Tests pass in both Debug and Release
7. [ ] Error messages follow: statement → analysis → advice
