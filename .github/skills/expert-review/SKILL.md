---
name: reviewing-compiler-prs
description: "Multi-agent, multi-model code review for F# compiler PRs. Dispatches parallel assessment agents across 15 F#-specific dimensions (type checking, IL emission, binary compatibility, AST correctness, parallel determinism, concurrency, IDE performance, diagnostics, feature gating, code quality, code reuse, complexity). Invoke for: code review, PR review, expert review, multi-agent review, panel review, diverse model review, review council, fellowship review."
---

# Reviewing Compiler PRs

Orchestrates a multi-model review panel across F#-specific dimensions. The full dimension definitions and CHECK rules live in the `expert-reviewer` agent — this skill handles dimension selection, the multi-model dispatch methodology, and self-review.

## When to Invoke the Agent

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

## Multi-Model Review Methodology

When thorough review is requested, the `expert-reviewer` agent uses this 3-phase approach:

### Phase 1: Briefing Pack

Build a self-contained briefing from:
1. PR metadata (title, body, labels)
2. Linked issues — fetch full body **and all comments** (real requirements live there)
3. Merge-base diff only — use `pull_request_read` → `get_diff` or `gh pr diff`
4. Changed files list with change types; test files called out separately
5. Commit messages

### Phase 2: Parallel Assessment

Dispatch **one agent per selected dimension**, each using the dimension's CHECK rules from the agent file. For high-confidence reviews, assess each dimension with multiple models (`claude-opus-4.6`, `gemini-3-pro-preview`, `gpt-5.2-codex`).

Each agent receives the briefing pack and returns findings with: location (file:line), description, severity (critical/high/medium/low).

**Assessment gates** (apply before flagging):
- **Execution context**: understand how and where the code runs. A test harness and the main compiler have different constraints.
- **Direction**: classify each change as regression, improvement, or unclear. Only regressions are findings.
- **Concrete scenario required**: report an ISSUE only when you can construct a specific failing scenario. No hypotheticals.

### Phase 3: Consolidation

1. **Deduplicate**: merge findings pointing at the same location with the same problem
2. **Filter false positives**: wrong context assumed → discard; improvement not regression → downgrade; speculation without evidence → LOW confidence
3. **Classify**: Behavioral (correctness) → Quality (structure) → Nitpick (style)
4. **Rank**: prefer findings flagged by more agents (cross-model agreement = higher confidence)
5. **Present**: Behavioral first, then Quality, then Nitpicks (only if no higher-level findings)

## Self-Review Checklist

Before requesting a full agent review:

1. [ ] Every behavioral change has a test
2. [ ] Test baselines updated with explanations for new errors
3. [ ] New language features have a `LanguageFeature` guard
4. [ ] No unintended public API surface changes
5. [ ] Cleanup changes separate from feature enablement
6. [ ] Compiler warnings resolved
7. [ ] Tests pass in both Debug and Release
8. [ ] Error messages follow: statement → analysis → advice
