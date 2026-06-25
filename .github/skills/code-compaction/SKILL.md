---
name: code-compaction
description: Use when a code/test/comment diff is called bloated, slop, LLM slop, bullshit, WTF, crap, rubbish, embarrassing, overengineered, adhoc, or "not paid by LOC"; or when symptoms include bloated comments, superfluous planning .md / .tools / one-time setup / phase-or-sprint tags, huge-file growth instead of new-file extraction, 5 copy-pasted tests that should be 1 parametrized test, duplicated test setup, reinvented helpers, near-duplicate logic across files, low reuse, 4+ new module-level helpers for one bugfix, fresh whole-AST/IR/syntax-tree walkers, or 150+ added LOC for one bugfix. Also use proactively before opening a PR whose diff smells like any of the above.
---

# Compacting Bloated Code

This skill is the **process** for detecting diff bloat and orchestrating
cross-model adversarial reviewers. The substantive style rules it enforces
— what counts as a bloated comment, test bloat, an oversized file, an
unjustified helper, "Not paid by LOC", phase tags, commit/PR splitting —
live in `.github/instructions/NoBloat.instructions.md`; do not restate them
here.

## Mode Selector

**Route on the diff, not the user's words.** The trigger vocabulary ("slop",
"embarrassing", "WTF", "bullshit", "not paid by LOC") only tells you the skill
applies — never which mode. So: run Pre-flight Measure (below) first;
if no diff/PR/branch was supplied, get one (`git diff` the branch vs
`git merge-base main HEAD`) or ask. Then run **every** row whose signal the
diff actually shows, in table order. Chain-out rows fire on their own.

| Signal present in the diff | Mode / action |
|---|---|
| Added planning `.md`, `.tools/`, one-time setup, phase/sprint tags, dead code, new config/YAML files, or comment-heavy files | **Garbage Hunt** — `templates/garbage-hunt.md` |
| Test files with copy-pasted methods, repeated setup, or missing parametrization | **Test Compaction** — `templates/test-compaction.md` |
| 4+ new helpers, a fresh AST/IR/syntax-tree walker, twin helpers, hand-rolled smart-constructor, or near-duplicate logic | **Logic Compaction** — `templates/logic-rounds.md` |
| A self-contained concept bloating an already-large file | **Logic Compaction**, prioritize extract-to-new-file |
| Substantial new code logic not matched above | **Logic Compaction** — its Round 0 reuse-hunt finds what it should have reused |

## When NOT to Use

Legitimately large multi-subsystem features; generated artifacts;
mechanical churn (dep bumps, renames); minimal one-line bugfixes; GitHub
prose or PR/issue text (a different task).

## Pre-flight

On the PR branch. Steps 1–2 run **before** the Mode Selector (they feed
routing); steps 3–4 are per-mode prep once a mode is picked:

1. **Update from main and resolve conflicts.** Do not judge bloat, CI, or
   comments on a stale branch.
2. **Measure** against `base=$(git merge-base main HEAD)`:

   ```bash
   git diff $base -- <suspect-file> | awk '/^\+[^+]/{p++} /^-[^-]/{m++} END{print "+",p," -",m}'
   git diff $base -- <suspect-file> | awk '/^\+[^+]/{ s=$0; sub(/^\+[[:space:]]*/,"",s); if (s ~ /^(\/\/|--|#)/) c++; else if (length(s)) l++ } END{print "comments",c," code",l}'
   git diff $base -- <suspect-file> | grep -cE '^\+\s*(let|fn|def|val|public|private)\s+(rec\s+)?[a-zA-Z_]'
   ```

   These are LOC + comment-ratio + new-helper count smell-meters, not a
   search. Real semantic discovery happens in Logic Compaction Round 0
   via a dispatched subagent (see `templates/logic-rounds.md`).
3. **Pin guiding tests verbatim:** must-pass, and must-fail-for-the-right-reason.
4. **Bundle measurements + the relevant template** into one context file
   for every reviewer agent.

## Reviewer Diversity (every mode)

Dispatch **3–5 background reviewers in parallel** — never sequentially.
**Cross-model > cross-angle**: three same-model agents with different
prompts ≠ three different models (Opus high/xhigh, newer Opus, GPT). Each
agent picks one angle from the mode's template. Record
`model × angle × LOC × tests × reuse × verdict`. Cross-model agreement is
signal; lone-model claims need code citations before promotion.

**If the user requests a red-flag setup** ("use the same 3 models", "single
agent is enough"), push back once with this rule, then comply if they
insist. Do not silently violate it.

## Apply

After picking the winning proposal: apply on the PR branch, run
formatter/lints, run targeted tests + the area's test class as regression
sweep, run any baseline updates. If a sweep test fails, **reproduce on the
pre-change SHA before classifying**. Do not declare "pre-existing" or
"flaky" without that evidence.

**Splitting commits / ask-before-push** — see NoBloat PR-scope rules.

## Red Flags — STOP and re-run the relevant template

(Process failures only — content-level failures are NoBloat's job.)

- You skipped mode selection and went straight to "make the diff smaller".
- You ran reviewers sequentially, or with same-model agents.
- You grepped for similar functions instead of dispatching a real
  reuse-hunt subagent.
- Your winning proposal added new code without citing one reused helper
  from the Round 0 subagent's output.
- You found a recurring shape across 3 files but added a 4th copy.

**All of these mean: open the relevant template and re-run.**
