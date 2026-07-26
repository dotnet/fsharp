# LabelOps

Opt-in, label-gated agentic workflows that keep open PRs healthy. Add a label to a PR → the agent checks it every 3 hours.

## Labels

| Label | Applied by | Effect |
|---|---|---|
| `AI-Auto-Resolve-Conflicts` | Maintainer | Merges `main` and resolves real conflicts (not mere behind-main). |
| `AI-Auto-Resolve-CI` | Maintainer | Triages CI failures: small fix, flake dispatch, or escalation. |
| `AI-needs-CI-fix-input` | Agent | Agent couldn't fix within budget; see escalation comment. |

All labels are sticky — the agent never removes them. Remove a label to opt out.

## How it works

Each run (every 3h): select up to 3 eligible PRs → for each: **CI first** (fix or escalate), **conflicts second** (only real conflicts). Pushing restarts CI, so only one action per PR per run.

Proven flakes (≥3 distinct PRs via `flaky-test-detector`) get dispatched to `labelops-flake-fix` — a separate workflow that opens a dedicated fix/quarantine PR.

## Guardrails

- Never modifies `.github/**`, never merges/approves/closes PRs.
- Never rebases or force-pushes. Merge commits only.
- Skips fork PRs, drafts, and `AI-Issue-Regression-PR` PRs.
- Every comment prefixed `🤖 *LabelOps — <subtopic>.*`.

## Setup (one-time)

```bash
gh label create "AI-Auto-Resolve-Conflicts" --repo dotnet/fsharp --color ededed \
  --description "Opt-in: LabelOps merges main and resolves conflicts every 3h"
gh label create "AI-Auto-Resolve-CI" --repo dotnet/fsharp --color ededed \
  --description "Opt-in: LabelOps triages CI failures every 3h"
gh label create "AI-needs-CI-fix-input" --repo dotnet/fsharp --color fbca04 \
  --description "LabelOps escalated: agent couldn't fix; see comment"
```

## Workflows

- [`.github/workflows/labelops-pr-maintenance.md`](../.github/workflows/labelops-pr-maintenance.md) — scheduled babysitter.
- [`.github/workflows/labelops-flake-fix.md`](../.github/workflows/labelops-flake-fix.md) — flake-fix spinoff (dispatch only).
