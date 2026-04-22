# LabelOps

LabelOps is a pair of opt-in, label-gated agentic workflows that help maintain
open PRs in `dotnet/fsharp`. Maintainers request help by adding a label to a
PR; the agent babysits every labeled PR every 3 hours until the maintainer
removes the label.

## Labels

| Label | Who applies | Effect |
|---|---|---|
| `AI-Auto-Resolve-Conflicts` | Maintainer | The next scheduled run will merge `main` into the PR branch and resolve any conflicts semantically. |
| `AI-Auto-Resolve-CI` | Maintainer | The next scheduled run will triage CI failures (proto build, build, test) and either apply a small fix, spawn a flake-fix PR, or escalate. |
| `AI-needs-CI-fix-input` | **Agent (adds only)** | The agent reproduced a failing test locally, couldn't fix it within a small-scope budget, and posted up to 3 options with tradeoffs. Humans take over. |

All three labels are **sticky**. The agent never removes a label; only
maintainers do.

## What the agent does

### Per PR, on every scheduled run

1. **CI first** — uses the `pr-build-status` skill to collect all errors
   across all platforms, then:
   - **All green** → no action, no comment.
   - **Small, mechanical fix** (missing `<Compile Include>`, fantomas format,
     baseline update, typo, missing `open`, out-of-date `.xlf`) → reproduce
     locally, apply, verify with `./build.sh -c Release` + the affected test,
     push, comment.
   - **Suspected flake** → run the `flaky-test-detector` skill. **Requires
     evidence across ≥3 distinct unrelated PRs.** If proven, the agent
     dispatches `labelops-flake-fix` (a separate workflow) to open a
     dedicated PR — your PR is left untouched. Comment on your PR explains.
   - **Design-level bug** (can't fix within ≤3 attempts / ≤500 LOC) →
     reproduce locally, add the `AI-needs-CI-fix-input` label, post an
     escalation comment with minimal repro, evidence, and up to 3 options.
     No code pushed.

2. **Conflicts second** — only if the CI step did not push (pushing restarts
   CI and discards the failure history). The agent runs
   `git merge origin/main`, resolves conflicts preferring both-entries for
   `.fsproj` `<Compile Include>` and `release-notes.md`, and pushes. No
   comment on a clean merge; a short comment lists resolved files otherwise.

### Opt-out

Remove the label. That's it.

### Guardrails

- Never modifies `.github/**`.
- Never merges, approves, closes, or reopens PRs.
- Never rebases or force-pushes.
- Never touches PRs from external forks or PRs labeled `AI-Issue-Regression-PR`
  (those are owned by `regression-pr-shepherd`).
- Max 5 PRs per run, shuffled seeded by `GITHUB_RUN_ID` for fairness.
- Max 3 flake-fix spinoffs per run.
- Every comment starts with `🤖 *LabelOps — <subtopic>.*`.

## Workflows

- [`.github/workflows/labelops-pr-maintenance.md`](../.github/workflows/labelops-pr-maintenance.md) — the scheduled babysitter.
- [`.github/workflows/labelops-flake-fix.md`](../.github/workflows/labelops-flake-fix.md) — the flake-fix spinoff, dispatched via `safe-outputs.dispatch-workflow` from the babysitter.

## Creating the labels

One-time setup (requires repo write):

```bash
gh label create "AI-Auto-Resolve-Conflicts" \
  --repo dotnet/fsharp \
  --color ededed \
  --description "Opt-in: LabelOps merges main into this PR and resolves conflicts every 3h"

gh label create "AI-Auto-Resolve-CI" \
  --repo dotnet/fsharp \
  --color ededed \
  --description "Opt-in: LabelOps triages CI failures on this PR every 3h"

gh label create "AI-needs-CI-fix-input" \
  --repo dotnet/fsharp \
  --color fbca04 \
  --description "LabelOps escalated: agent reproduced a failure but couldn't fix it; see comment for options"
```

## Operational notes

- **Schedule initially disabled.** The first landing of these workflows ships
  with only `workflow_dispatch` available. A follow-up PR enables the 3h
  schedule once maintainers have validated the behavior via a manual dispatch
  on one or two volunteer PRs.
- **Re-engagement after escalation.** When a PR carries
  `AI-needs-CI-fix-input`, CI work is skipped until the PR's head commit is
  newer than the escalation comment. Push a new commit and the agent
  retries. Conflict work is never blocked by this label.
- **No persistent memory.** Everything the agent decides is derived from
  signals already on the PR (labels, commit SHAs, comment timestamps).
