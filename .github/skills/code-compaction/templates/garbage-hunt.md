# Garbage Hunt (deletion-only)

No redesigns, no new helpers, no hoisting (Test Compaction's job).
Delete bytes that don't change test outcomes or remove docs a downstream
consumer reads.

## Dispatch — 3 cross-model adversarial reviewers per SKILL.md "Reviewer Diversity"

```
You are an adversarial garbage-hunt reviewer. Mandate is DELETION ONLY.
No redesigns, no new helpers, no hoisting.

INPUTS:
1. <full PR diff>
2. <repo's NoBloat / style instruction file if present — AUTHORITATIVE
   on which comments / scaffolding / tests to delete and which to keep;
   apply its rules verbatim>

DELETE per the style file. Process-level additions beyond the style file:
- planning / progress `.md` files (`*plan*.md`, `*status*.md`, `*notes*.md`)
- anything under `.tools/`
- one-time setup files or scripts not part of the product
- "phase", "sprint", "transitional", "follow-up", "scaffold" tags in
  commits, comments, or file names
- `[<Fact>]` / `it(...)` with empty body or `Assert.True(true)`
- unused `open` / `import` / `using`
- superfluous YAML / config blocks added only to support the agent's
  workflow rather than the product

DELIVERABLES:
1. Unified deletion diff (real code).
2. Per-deletion table: file:line, bytes deleted, risk class
   (none / low / medium), test/build command that proves it is safe.
3. List of items KEPT and why (one line each) — your defense against
   "you deleted a useful comment".
```

## Consolidate

- **Unanimous deletes** — apply automatically; verify with cited test commands.
- **Single-reviewer deletes** — ask the user before applying.
- **Conflicting verdicts** — keep the item; document the disagreement.

After Garbage Hunt, re-measure. If LOC is still high, the remaining bloat
is structural — move to Logic Compaction.
