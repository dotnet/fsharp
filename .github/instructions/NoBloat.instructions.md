---
applyTo:
  - "src/Compiler/**/*.{fs,fsi}"
  - "vsintegration/src/**/*.{fs,fsi}"
  - "tests/FSharp.Compiler.ComponentTests/**/*.fs"
  - "tests/FSharp.Compiler.Service.Tests/**/*.fs"
  - "vsintegration/tests/**/*.fs"
---

# Code Style: No Bloat

Reviewers read code, not prose. Add bytes only when they pay for themselves.

## Comments

Good names beat comments **24/7**. Before writing a comment, ask: *can I rename a value, extract a function, or use an active pattern so the comment becomes unnecessary?* If yes, do that instead.

- **Do not** restate what the code says (variable name, type name, attribute name, function signature).
- **Do not** narrate the algorithm step-by-step. The diff is the algorithm.
- **Do not** justify design decisions inline ("we chose X over Y because…"). Put rationale in the commit message or PR body.
- **Do not** leave war-story comments ("previously we did Z, but…", "counter-example: …"). The history is in `git log`.
- **Do not** write multi-line `///` doc comments for internal helpers whose body is one expression.

Acceptable comments answer **why**, not **what**, and only when the *why* is non-obvious and cannot be expressed by renaming:
- Workarounds for compiler/runtime bugs (link the bug).
- Performance constraints invisible from the code shape ("inner loop runs 50M times per typecheck").
- Cross-file invariants the code itself can't enforce.

If you are tempted to write `// This is intentional`, change the code so the intent is structural, not decorative.

## Code shape

- Compact, idiomatic F#: pattern matching over `if`/`elif` ladders; active patterns where they remove duplication.
- Low cyclomatic complexity per function. Extract helpers — even one-line ones — when a name clarifies a step.
- Prefer module-level `let` over big bodies with nested locals.
- New file > bloating an existing 5000-line file when adding a self-contained concept.

## Test code

Tests get a touch more leeway for explanation, but the same rules largely apply:

- One parametrized test (`[<Theory>]`, `[<InlineData>]`, or a `for/yield` over inputs) > five copy-pasted tests.
- Module-level constants for shared paths (`Path.Combine` for OS neutrality), shared source strings, and shared expected outputs.
- Helpers like `parseAndCheck code` over reinventing the setup per test.
- Don't reinvent an entire `.fs` file inside each test when one shared module-level binding will do.

## PR scope

**Not paid by LOC.** Large PRs are typically shitty PRs. If the diff has 1000+ lines, split it.
- Cleanup commits separate from feature commits.
- No "phase tag" / "transitional measure" / "follow-up" comments left behind — either do it now in a follow-up commit, or file an issue. Don't leave breadcrumbs in the code.

## When in doubt

Delete the comment, rename the value, and re-read. If the code is still unclear, *that* is what needs fixing — not the comment.
