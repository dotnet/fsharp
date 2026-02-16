---
name: review-council
description: "Multi-agent review council. Invoke for: review council, fellowship review, multi-agent review, panel review, diverse model review."
---

# Review Council

A multi-model code review panel. You orchestrate 3 phases: build a briefing pack, dispatch 15 parallel assessment agents (5 dimensions × 3 models), and consolidate findings.

## Phase 1: Briefing Pack

Auto-detect the PR from the current branch. Build a self-contained briefing document containing all of the following:

1. **PR metadata**: title, body, labels
2. **Linked issues**: for every issue referenced in the PR body or commits, fetch the full issue body and **all comments** — this is where the real requirements and context live
3. **Full diff**: merge-base diff only (`git diff $(git merge-base HEAD <base>)..HEAD`) — shows only what the PR adds, not what main gained since the branch point
4. **Changed files list**: with change type (added/modified/deleted), test files called out separately
5. **Commit messages**: useful for correlating claims to specific code changes, but not a source of requirements

Use `claude-opus-4.6` for this phase. The output is a single structured briefing document. Every assessment agent in Phase 2 receives this briefing verbatim — they should not need to re-fetch any of this context.

## Phase 2: Parallel Assessment

Dispatch **15 agents in parallel**: each of the 5 dimensions below assessed independently by each of 3 models.

**Models**: `claude-opus-4.6`, `gemini-3-pro-preview`, `gpt-5.2-codex`

Each agent receives:
- The full briefing pack from Phase 1
- Its specific dimension prompt (below)
- Instruction to return findings as a list, each with: location (file:line), description, severity (critical/high/medium/low)

---

### Dimension 1: Claims Coverage

Are the goals met? Cross-reference every claim in the PR description, linked issues, and commit messages against actual code changes. Flag:

- **Orphan claims**: stated in PR/issue but no corresponding code change implements it
- **Orphan changes**: code changed but not mentioned in any claim — why is this here?
- **Partial implementations**: claim says X, code does X-minus-something — what's missing?

---

### Dimension 2: Test Coverage

Is every feature, fix, or behavioral change covered with tests? Check for:

- **Happy path**: does the basic intended usage work and is it tested end-to-end?
- **Negative path**: what happens with invalid input, malformed syntax, error conditions? Are diagnostics tested?
- **Feature interactions**: this is a compiler — edge cases are about how the change interacts with other F# features. Example: a codegen change should test both reference types and value types. A syntax change should test interaction with generics, constraints, computation expressions, etc.
- **Assertion quality**: tests must actually assert the claimed behavior, not just "compiles and runs without throwing". A test that calls the function but doesn't check the result is not a test.

Flag any behavioral change in the diff that lacks a corresponding test. Tests should be in the appropriate layer — pick based on what the issue is and what changed:
- **Typecheck tests**: the bulk of coverage — type inference, constraint solving, overload resolution, expected compiler warnings and errors
- **SyntaxTreeTests**: parser/syntax changes
- **EmittedIL tests**: codegen/IL shape changes
- **compileAndRun tests**: end-to-end behavioral correctness that absolutely needs proper execution on the .NET runtime
- **Service.Tests**: FCS API, editor features
- **FSharp.Core.Tests**: core library changes

A PR can and often should have tests in multiple categories.

---

### Dimension 3: Code Quality

Assess structural quality of the changes:

- **Logical layer placement**: is the code in the right module/file, or shoved somewhere convenient? Would a reader expect to find this logic here?
- **Ad-hoc "if/then" patches**: flag any `if condition then specialCase else normalPath` that looks like a band-aid rather than a systematic fix. These are symptoms of not understanding the root cause — the fix should be at the source, not patched at a consumer. A conditional that exists only to work around a bug elsewhere is a code smell.
- **Duplicated logic within the PR diff**: same or near-same code appearing in multiple places in the changeset
- **Error handling**: not swallowing exceptions, not ignoring Result values, not using `failwith` where a typed error would be appropriate

---

### Dimension 4: Code Reuse & Higher-Order Patterns

Search the codebase for existing patterns that match the new code's structure. F# allows extracting logic into composable pieces — mappable structures, foldable, walkers, visitors. Look for:

- **Highly similar nested pattern matches**: a familiar structure of nested `match ... with` but with a minor tweak. This is the #1 symptom — slight differences can almost always be extracted into a higher-order function or otherwise parameterized or made generic.
- **Copy-paste-modify**: new code that duplicates an existing function with small changes. The difference should be a parameter, a generic type argument, or a function argument (higher-order function).
- **Missed abstractions**: where two pieces of code share structure but differ in a specific operation — that operation should be a parameter, a generic type argument, or a function argument.
- **Existing utilities ignored**: the codebase may already have helpers, combinators, or active patterns that do what the new code reimplements from scratch. Search for them.

---

### Dimension 5: Cyclomatic Complexity

Assess complexity of added/changed code:

- **Pyramid of nested doom**: deeply nested `if/then/else`, heavily nested `match`or interleaving with `for` and other branching constructs. Any nesting beyond 2 levels should be questioned — is there a flatter way?
- **F# offers better tools**: pattern matching and active patterns for non-trivial branching logic — flatter and easier to read than chains of `if/elif/else`. Suggest them as alternatives.
- **Active patterns for complex conditions**: when a match guard or if-condition encodes domain logic, an active pattern names it and makes it reusable.
- **Pipelines over nesting**: sequential operations should be pipelined, not nested. Collections, Result, Option, ValueOption all support this — use `|>`, `bind`, `map` chains instead of nested `match` or `if/then`.
- **High branch count**: functions with many `match` arms or `if` branches — consider whether the cases can be grouped, or whether the function is doing too much and should be split.
- **Flatter is better**: a flat pattern match with 10 arms is easier to read than 4 levels of nesting with 10 combinations. Prefer wide over deep.

---

## Phase 3: Consolidation

Collect all findings from the 15 agents. Then:

1. **Deduplicate**: multiple agents will find the same issue. Merge findings that point at the same location and describe the same problem. Keep the best-written description.
2. **Classify** into three buckets:
   - **Behavioral**: missing feature coverage, missing tests, incorrect logic, claims not met — things that affect correctness
   - **Quality**: code structure, readability, complexity, reuse opportunities — it works, but could be better
   - **Nitpick**: typos, naming, formatting, minor style — agents love producing these to have *something* to say. Low priority. Only surface if there are no higher-level findings.
3. **Rank within each bucket**: prefer findings flagged by more agents (cross-model agreement = higher confidence).
4. **Present**: Behavioral first, then Quality, then Nitpicks (if any). For each finding: location, dimension, description, how many agents flagged it.

If a model is unavailable at runtime, proceed with the remaining models. Minimum viable council = 2 models.
