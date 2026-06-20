---
name: reviewing-compiler-prs
description: "Performs multi-agent, multi-model code review of F# compiler PRs across 19 dimensions including type checking, IL emission, binary compatibility, and IDE performance. Dispatches parallel assessment agents per dimension, consolidates with cross-model agreement scoring, and filters false positives. Invoke when reviewing compiler changes, requesting expert feedback, or performing pre-merge quality checks."
---

# Reviewing Compiler PRs

Full dimension definitions and CHECK rules live in the `expert-reviewer` agent.

## When to Invoke

- PR touches `src/Compiler/` — invoke the `expert-reviewer` agent
- PR touches `src/FSharp.Core/` — focus on FSharp.Core Stability, API Surface, Backward Compat, XML Docs
- PR touches `vsintegration/` or `LanguageServer/` — focus on IDE Responsiveness, Concurrency, Memory
- PR touches `tests/` only — quick check: baselines explained? Cross-TFM coverage? Tests actually assert?
- PR touches `eng/` or build scripts — focus on Build Infrastructure, Cross-Platform

## Dimension Selection

| Files Changed | Focus Dimensions |
|---|---|
| `Checking/`, `TypedTree/` | Type System, Overload Resolution, Struct Awareness, Feature Gating |
| `CodeGen/`, `AbstractIL/` | IL Emission, Debug Experience, Test Coverage |
| `Optimize/` | Optimization Correctness, IL Emission, Test Coverage |
| `SyntaxTree/`, `pars.fsy` | Parser Integrity, Feature Gating, Typed Tree Discipline |
| `TypedTreePickle.*`, `CompilerImports.*` | Binary Compatibility (highest priority) |
| `Service/` | FCS API Surface, IDE Responsiveness, Concurrency, Incremental Checking |
| `LanguageServer/` | IDE Responsiveness, Concurrency |
| `Driver/` | Build Infrastructure, Incremental Checking, Cancellation |
| `Facilities/` | Feature Gating, Concurrency |
| `FSComp.txt` | Diagnostic Quality |
| `FSharp.Core/` | FSharp.Core Stability, Backward Compat, XML Docs, RFC Process |
| `vsintegration/` | IDE Responsiveness, Memory Footprint, Cross-Platform |
| `eng/`, `setup/`, build scripts | Build Infrastructure, Cross-Platform |

## Subagent Dispatch

For each selected dimension from the table above, the `expert-reviewer` agent MUST launch an independent subagent (background task) to assess that dimension. This is not optional — a single agent doing all dimensions sequentially produces shallow analysis and wall-of-text summaries.

Each subagent receives:
1. The dimension's CHECK rules (from expert-reviewer.md)
2. The relevant file diffs (filtered by the dimension's hotspot paths)
3. Instructions to produce a structured finding: `{file, line, severity, dimension, issue, suggestion}` or `LGTM` if no findings

The expert-reviewer consolidates subagent results, deduplicates, applies assessment gates, and posts as inline comments per Wave 5.

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
2. [ ] FSharp.Core changes maintain binary compatibility
3. [ ] No unintended public API surface changes
4. [ ] New language features have a `LanguageFeature` guard and RFC
5. [ ] No raw `TType_*` matching without `stripTyEqns`
6. [ ] Cancellation tokens threaded through async operations
7. [ ] Cleanup changes separate from feature enablement

Full dimension CHECK rules are in the `expert-reviewer` agent.
