# Logic Compaction — Adversarial Process

Rounds: **0 Reuse Hunt → 1 Diverge → 2 Adversarial → 3 Prototype**.

## Round 0a — Measure (smell-meters, not search)

Logic-specific smell-meters against `base=$(git merge-base main HEAD)`
(SKILL.md pre-flight covers the universal ones):

```bash
# Walker smell — fresh case-by-case enumeration of a union/sum type.
git diff $base -- <suspect-file> | grep -cE '^\+\s+\|\s+[A-Z]\w*\.[A-Z]'
# Hand-rolled domain-node construction smell.
git diff $base -- <suspect-file> | grep -cE '^\+\s+(new |Expr\.|Stmt\.|Node\.|Pat\.|Type\.)'
```

These produce a number. High counts (≥20) flag the pattern. They are
NOT a search for existing helpers — that comes next.

## Round 0b — Reuse Hunt (subagent-driven)

**Do not grep for similar functions.** Substring grep finds matches by
*name*; it misses functions that match by *shape* but not by name —
which is the exact reuse the bloated patch should have used. Dispatch a
real subagent with the agent's full toolset (code intelligence,
semantic search, LSP, symbol search). **Spend the tokens** — a thorough
reuse hunt up front saves Round 1 reviewers from proposing new code
that duplicates existing infrastructure.

### Subagent dispatch prompt

```
You are a reuse-hunt subagent for a code-compaction round.

CONTEXT:
1. <full bloated PR diff>
2. <2-sentence description of what the new code DOES at the type+effect
   level — not what it adds line-by-line, but what it computes/transforms>
3. <list of new helpers the PR adds + their full signatures>

YOUR JOB:
1. Find existing helpers / extractors / smart constructors / active
   patterns / fold visitors / traversals in this codebase that ALREADY
   do something close enough that the bloated code should reuse or
   extend them. Use whatever search tools you have — code intelligence,
   semantic search, symbol search, LSP. Do NOT rely on substring grep
   alone; grep misses functions that match by shape but not by name.
2. For each match: cite file:line, the signature, ONE example caller,
   and a 1-sentence relevance note ("exact reuse", "close enough with
   one extra parameter", "same shape but inverted", …).
3. If you find ≥2 existing functions with the same shape modulo one
   lambda or type parameter, flag a missing higher-order abstraction:
   the bugfix should become the 3rd caller of a new HOF (not the
   4th copy). Sketch the HOF signature.
4. Report the negative space: what you searched for, what came up
   empty. Round 1 reviewers need to know what's actually new vs
   reinvented.

Spend the tokens. Recurse into multiple search strategies if the first
one returns weak matches.

OUTPUT:
- list of reuse candidates with file:line and relevance note
- any "waiting to be invented" HOF proposals with signature + ≥2 existing
  callers to port
- list of what you searched for and found nothing
```

The subagent's output IS the Round 0 context file every Round 1 reviewer
reads. If it finds reuse candidates, the bloated patch is almost
certainly avoidable — feed those candidates into Round 1 as the
strongest angle.

## Round 0c — Pin guiding tests

- **Must-pass tests** — the bugfix's positive cases.
- **Must-fail-for-the-right-reason tests** — regression guards.
  Re-examine each: was it a workaround for the wrong invariant? The
  elegant fix may make its failure mode impossible by construction.

## Round 1 — DIVERGE (3–5 reviewers, model diversity first)

Each agent defends ONE angle as the strongest possible:

1. **Reuse a Round-0 candidate** the subagent found (most common winner).
2. **Extract a missing HOF** (the "waiting to be invented" case from Round 0).
3. **Rewrite-and-re-enter:** normalize input and recurse into the existing
   translator/visitor instead of duplicating its semantics.
4. **Make the bad case structurally impossible** (no runtime guard needed).
5. **File-pressure:** extract the new concept to a new focused file rather
   than adding another block to the giant file.

### Round 1 dispatch template

```
You are a rubber-duck reviewer. The PR under review is overengineered.

READ FIRST (in order):
1. <Round 0 context: smell-meter numbers, REUSE-HUNT subagent output,
   guiding-test contracts, bloated-diff snippet>
2. <surrounding-code file:line ranges>
3. <test file:line ranges: must pass / must keep failing>

YOUR ASSIGNED ANGLE: **<angle name + 2-sentence premise>**

CONSTRAINTS:
- ≤ <N> LOC of new code (N = 1/4 of current bloat).
- 0 new module-level helpers, OR justify each (≥2 real callers OR a
  single-use helper that NAMES a non-evident operation).
- No new case-by-case walkers when the codebase has an existing
  visitor/fold; the Round 0 subagent has the list.
- No hand-rolled domain-node construction when a smart-constructor
  module exists.
- No hand-rolled list/option/result/state plumbing when the prelude has it.
- Must pass tests <list>; must still fail (for the right reason) tests <list>.
- MUST cite ≥1 reuse candidate from the Round 0 subagent's output with
  file:line. If you propose a new HOF, the Round 0 subagent must have
  flagged the recurring shape — if it didn't, you don't have a recurring
  shape, only a guess.

AGGRESSIVE QUESTIONING (run every one before writing the diff):
- Is any guard there to compensate for the wrong invariant? (state the
  invariant; if the guard restores it, fix the invariant instead)
- Could one generic combinator (fold / map / iter / bind) replace 10+
  lines of hand-rolled control flow?
- Is the helper's signature missing a generic that would make it
  reusable for ≥2 existing sites?
- Can the bloated function be split into "pure rewrite (HOF)" + "decide
  whether to rewrite (predicate)" and both parts already exist?
- Can the must-keep-failing test be deleted because the elegant fix
  makes its failure mode impossible by construction?

DELIVERABLES:
1. Unified diff (real code, not pseudocode).
2. LOC count of new code.
3. Count of new module-level helpers + per-helper justification.
4. List of REUSED helpers / active patterns from Round 0, with file:line.
5. (If proposing a new HOF) signature + the 2+ existing callers to port
   (from Round 0's recurring-shape flag).
6. Position on each guard test (keep / change-expectation / delete + why).
7. ≤ 250-word justification.
```

## Round 2 — ADVERSARIAL (3 reviewers attack top 3 proposals)

Pick the **top 3 from Round 1** by `tests-green × LOC × reuse-citation`.
Dispatch 3 new cross-model reviewers in parallel — each takes ONE
proposal and writes the strongest counter-argument.

### Round 2 dispatch template

```
You are an adversarial reviewer. Your job is to KILL the proposal below.

READ FIRST:
1. <Round 0 context, including REUSE-HUNT subagent output>
2. <surrounding code>
3. <tests>
4. <FULL Round 1 proposal — diff + justification — verbatim>

ATTACK ANGLES:
- Construct compiling inputs/tests that break semantics or
  escape/over-trigger the rewrite.
- Find a reuse candidate the Round 0 subagent missed. If you find one,
  explain how the subagent's search strategy missed it; that's a Round
  0 subagent improvement.
- Find a reuse candidate from Round 0's output that the proposal IGNORED.
- If a helper was added, prove it should inline OR show ≥2 real callers
  (the Round 0 subagent should have found them; if not, the helper is
  speculative).
- If no helper was added, look for the missed "waiting to be invented"
  HOF the Round 0 subagent flagged.
- Check binding scope, evaluation order, diagnostics, output order,
  output format, debug info.

DELIVERABLES:
1. Concrete failure cases with code/input snippets that compile.
2. Missed reuse opportunities (file:line — from Round 0 OR new ones with
   proof of how the Round 0 search strategy missed them).
3. Verdict: SURVIVES / NEEDS-FIX / REPLACE-WITH-X.
4. If REPLACE-WITH-X: full unified diff of your alternative + reuse citations.
5. ≤ 250-word verdict justification.
```

## Round 3 — PROTOTYPE (real branches, real tests, real LOC)

For each Round 2 survivor, on a real local branch:

```bash
git checkout -b proto-<angle-tag>
# Apply the proposal as written. Build clean? Lints clean?
# Run must-pass tests; must-keep-failing tests still fail for the RIGHT
# reason; broader regression sweep of the bug's area.
git diff <PR-head-sha> -- <files> \
  | awk '/^\+[^+]/{p++} /^-[^-]/{m++} END{print "+",p," -",m}'
```

Every prototype reports the same row:

| Proposal | LOC vs PR head | New helpers (n + justified?) | Reused helpers (file:line) | New walkers | Must-pass | Regression sweep | Build/lint warnings |

## Pick

Score: `tests-green × LOC × reuse-citation-quality`.

**Hard vetoes** (any violation of Round 1 CONSTRAINTS):
- fresh case-by-case walker over a union type (unless it composes an
  existing visitor/fold);
- hand-rolled domain-node construction when a smart-constructor module
  exists;
- hand-rolled list/option/result plumbing where the prelude has it;
- new code with zero reuse citation from Round 0.

Hand back to SKILL.md "Apply".
