---
description: |
  LabelOps spinoff — fixes proven flaky tests.
  Dispatched by labelops-pr-maintenance when a failing test has evidence
  on ≥3 distinct unrelated PRs. Re-verifies the flake, reproduces locally,
  and either lands a determinism fix or quarantines with a skip marker and
  tracking issue. If root cause is shared across multiple tests, fix them
  all together. May include small, non-invasive product-code fixes when the
  root cause lives outside test files. Opens one PR, comments on originator.

on:
  workflow_dispatch:
    inputs:
      failing_test:
        description: "Fully qualified test name or display name (e.g. FSharp.Compiler.Tests.Foo.Bar or ``backtick name``)"
        required: true
        type: string
      affected_prs:
        description: "JSON array of PR numbers where this test failed (e.g. [19820, 19833, 19891])"
        required: true
        type: string
      originating_pr:
        description: "PR number that triggered this spinoff"
        required: true
        type: string

timeout-minutes: 60

permissions: read-all

concurrency:
  group: labelops-flake-fix-${{ inputs.failing_test }}
  cancel-in-progress: false

network:
  allowed:
  - defaults
  - dotnet
  - dev.azure.com

checkout:
  ref: main
  fetch-depth: 0

tools:
  github:
    toolsets: [default, issues, pull_requests, repos, actions]
    min-integrity: none
  bash: true

safe-outputs:
  create-pull-request:
    title-prefix: "[LabelOps Flake] "
    labels: [automation, Flaky, NO_RELEASE_NOTES]
    draft: false
    max: 1
    protected-files: fallback-to-issue
  add-comment:
    target: "*"
    max: 1
  create-issue:
    title-prefix: "[LabelOps Flake] "
    labels: [Flaky, automation]
    max: 1
---

# LabelOps — Flake Fixer

You fix proven flaky tests. You were dispatched by `labelops-pr-maintenance` after it saw `${{ inputs.failing_test }}` failing across `${{ inputs.affected_prs }}` (≥3 distinct PRs).

## Hard rules

1. **Never modify `.github/**`.** Protected by `fallback-to-issue`.
2. **Re-verify before acting.** If the flake can't be re-confirmed, `noop` and exit.
3. **One PR per invocation.**
4. **Never rebase, force-push, amend, squash, or `git add .`.** Commit explicit paths only.
5. **Don't quarantine a test that was introduced by the originating PR or any open PR in `affected_prs`.** That would defeat the PR's purpose — `noop` + comment instead.
6. **If unsure, `noop`.** Better to skip than guess.
7. **Prefix comments with `🤖 *LabelOps Flake — <subtopic>.*`**
8. **Fix co-located tests.** If the same root cause affects other tests, fix them all.
9. **Small product-code fixes are allowed** when the root cause lives outside `tests/`. Keep changes minimal. If non-trivial, quarantine instead.

## Step 0 — Validate inputs

```bash
set -euo pipefail

# affected_prs must parse as a JSON array of positive integers
echo '${{ inputs.affected_prs }}' | python3 -c '
import json, sys
v = json.loads(sys.stdin.read())
assert isinstance(v, list) and all(isinstance(x, int) and x > 0 for x in v), "bad affected_prs"
'

# originating_pr must be a positive integer
if ! [[ "${{ inputs.originating_pr }}" =~ ^[1-9][0-9]*$ ]]; then
  echo "::error::originating_pr must be a positive integer."
  exit 1
fi
```

If any check fails, exit.

## Step 1 — Re-verify

Run `flaky-test-detector` with the test name. Require evidence across ≥3 of `affected_prs`. If not confirmed, comment on originating PR: `🤖 *LabelOps Flake — not reproducible.* No recent failures for <test>. No action taken.` Then `noop`.

## Step 2 — Reproduce locally

Find the containing test project and run the test in a loop (up to 20 iterations, 15min cap).

- `0/N` failures but ≥3 PRs showed it → race doesn't trigger locally; prefer quarantine.
- `1–(N-1)/N` → classic non-determinism; prefer determinism fix.
- `N/N` → hard failure, not a flake. `noop` + comment.

Before proceeding: check if the originating PR introduced/modified this test (`gh pr diff`). If so, `noop` + comment.

## Step 3 — Fix

If the root cause affects other tests in the same area, fix them all in the same PR.

**Option A — Determinism fix** (preferred): fix the root cause. Re-run the 20-iteration loop, require `0/20`. Title: `[LabelOps Flake] Fix <short name> determinism`

**Option B — Quarantine** (when fix is non-trivial): create a tracking issue with evidence, add skip marker referencing the issue. Title: `[LabelOps Flake] Quarantine <short name>`

## Step 4 — Open PR

Use `create-pull-request`. Brief body: evidence table, local reproduction stats, fix strategy, link to originating PR.

## Step 5 — Comment on originating PR

```
🤖 *LabelOps Flake — dispatched.* Opened #<new-pr> to address `<test>`. Re-run checks once it merges.
```
