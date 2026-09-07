---
name: feature-completeness
description: Use when about to defer useful work — when you catch yourself writing "follow-up", "future work", "out of scope", "v2", "next phase", "tracked in #NNN", "left as a TODO", "known limitation", or proposing to file an issue instead of finishing. Forces a binary decision: do the work now because it meets the product bar, or drop it with a real reason. Applies to feature scope, edge cases, missed optimizer/codegen cases, diagnostics gaps, and half-implemented APIs.
---

# Feature Completeness — No Deferral of Useful Work

"Follow-up" is not a word in our book. Neither is "v2", "next phase", "future work", or "tracked in a ticket". A tracker is not a decision — it is a place decisions go to die.

For any piece of work you are tempted to defer, you have exactly **two** legal outcomes:

1. **DO IT** — because the code you are already adding justifies it and it meets the product bar. Finish it now, in this change.
2. **DROP IT** — because it genuinely does not meet the product bar. Say *why* in one sentence. "Not important enough" is a valid reason; "would be more work" is not.

There is no third outcome. "File a follow-up issue" is never the resolution to useful work.

## The test

Ask: *does the code I am already writing justify this case?* If a transform, feature, or fix already handles shape A, and shape B is the same shape with the gate flipped, then B is not a follow-up — it is the same feature, half-done. Leaving B out is shipping a **half-arsed feature**, not scoping.

If B is genuinely a different, larger piece of work with its own design and risk surface — that is a real reason to draw a line. But then say so concretely ("B needs its own RFC / changes the pickle format / reopens a rejected design"), not "follow-up".

## Red flags (stop and decide)

- "I'll add a follow-up for the other cases."
- "Out of scope for this PR" — for something one gate-flip away.
- "Left as a known limitation / TODO."
- "v2 / next phase / future work."
- "Tracked in #NNN" used as a substitute for doing or dropping.
- A guard/gate whose only justification is "keeps the diff small" while excluding cases the mechanism already handles correctly.

## How to resolve one

1. **Measure or reason** whether the deferred case actually pays off. Get the number (closures removed, allocations saved, cases fixed). Don't guess.
2. **Try it.** Flip the gate, run the full targeted test suite, diff baselines. Half the time "follow-up" work is a five-line change that just needed someone to do it.
3. **If it's good** (meets the bar, tests green, no regression) → keep it. Done, not deferred.
4. **If it's not** → delete it and state the product-bar reason. If it was never important enough to do, it was never important enough to track.

## Why not "just file an issue"?

Because an unfinished feature with a tracking issue is worse than either a finished feature or a cleanly-scoped one:

- The next reader hits the gap, not the issue.
- The issue rots; nobody owns the other half.
- It hides an unmade decision behind process.

Decide now. Ship complete work, or ship a smaller thing you can stand behind.

## Relationship to NoBloat

`.github/instructions/NoBloat.instructions.md` bans "phase tag / transitional measure / follow-up" **comments** in code. This skill is the same principle one level up: it bans deferring the **work itself**. NoBloat says don't leave the breadcrumb; this says don't leave the gap.

Completeness is not the enemy of a minimal diff. A minimal diff does one thing *fully*. Bloat is doing unrelated things; a half-feature is doing one thing *partially*. Avoid both.
