---
description: |
  LabelOps spinoff — fixes a single proven flaky test.
  Dispatched by labelops-pr-maintenance via safe-outputs.dispatch-workflow
  when a failing test has been observed on ≥3 distinct unrelated PRs.
  Re-verifies the flake, reproduces it locally (a loop of 20 iterations),
  and either lands a determinism fix or quarantines the test with a skip
  marker linking to a tracking issue. Opens exactly one PR scoped to
  tests/** and vsintegration/tests/**, then comments on the originating PR.
  Never touches production code. Never runs on its own schedule.

on:
  workflow_dispatch:
    inputs:
      failing_test:
        description: "Fully qualified test name (e.g. FSharp.Compiler.Tests.Foo.Bar)"
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
    allowed-files:
      - "tests/**"
      - "vsintegration/tests/**"
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

You fix **one** proven flaky test per invocation. You were dispatched by `labelops-pr-maintenance` after it observed the test failing on `${{ inputs.affected_prs }}` — at least 3 distinct unrelated PRs.

## Inputs

- `failing_test`: `${{ inputs.failing_test }}`
- `affected_prs`: `${{ inputs.affected_prs }}` (JSON array)
- `originating_pr`: `${{ inputs.originating_pr }}`

## Absolute rules

1. **Scope is strictly `tests/**` and `vsintegration/tests/**`.** `allowed-files` enforces this on `create-pull-request`. If the fix would require production-code changes, do **not** touch production code — quarantine instead (Option B below).
2. **Never modify `.github/**`.**
3. **Re-verify before acting.** If the flake can't be re-confirmed, emit `noop` and exit.
4. **One PR per invocation.**
5. **Never rebase, force-push, amend, squash, or run `git add .`.** Always commit explicit paths.
6. **Never quarantine or modify a test that was introduced or changed by `originating_pr` or any still-open PR in `affected_prs`.** Quarantining such a test defeats the PR's purpose — the PR author added it intentionally. Skip to `noop` + comment if this is the case.
7. **If unsure, prefer `noop` over a wrong action.**
8. **Always prefix comments with `🤖 *LabelOps Flake — <subtopic>.*`**.

## Step 0 — Validate inputs (mandatory before any other step)

```bash
set -euo pipefail

# failing_test must match a conservative FQN charset
if ! [[ "${{ inputs.failing_test }}" =~ ^[A-Za-z0-9._+\-]+$ ]]; then
  echo "::error::failing_test does not match expected pattern; aborting."
  exit 1
fi

# affected_prs must parse as a JSON array of positive integers
echo '${{ inputs.affected_prs }}' | python3 -c '
import json, sys
v = json.loads(sys.stdin.read())
assert isinstance(v, list) and all(isinstance(x, int) and x > 0 for x in v), "affected_prs must be a JSON array of positive ints"
'

# originating_pr must be a positive integer
if ! [[ "${{ inputs.originating_pr }}" =~ ^[1-9][0-9]*$ ]]; then
  echo "::error::originating_pr must be a positive integer."
  exit 1
fi
```

If any check fails, exit the run — do not invoke the LLM.

## Process

### Step 1 — Re-verify the flake

Run the `flaky-test-detector` skill with `failing_test`. Require evidence across ≥3 of the `affected_prs`. If the skill no longer finds evidence (the test may have been fixed on `main` since dispatch), post on the originating PR:
```
🤖 *LabelOps Flake — not reproducible.* Re-check of `<test>` found no recent failures. No action taken.
```
Emit `noop` and exit.

### Step 2 — Reproduce locally

```bash
set -euo pipefail
FAILING_TEST='${{ inputs.failing_test }}'

# Determine the containing test project by enumerating candidate test projects
# and asking the test host which one declares the exact FQN. This avoids the
# footgun of grepping test-name fragments (which can silently pick the wrong
# project when the same short name appears in multiple files).
PROJ=""
for candidate in $(find tests vsintegration/tests -maxdepth 4 -name '*.fsproj' 2>/dev/null); do
  if dotnet test "$candidate" -c Release --no-build --list-tests 2>/dev/null | grep -Fqx -- "    $FAILING_TEST"; then
    PROJ="$candidate"
    break
  fi
done

if [[ -z "$PROJ" ]]; then
  echo "::warning::Could not locate test project for FQN $FAILING_TEST via --list-tests. Aborting."
  exit 0  # soft-exit → agent will emit noop
fi
echo "Project: $PROJ"

# Run the test up to 20 times or 15 minutes, whichever first.
FAILS=0
RUN=0
DEADLINE=$(( $(date +%s) + 15 * 60 ))
while [[ $RUN -lt 20 && $(date +%s) -lt $DEADLINE ]]; do
  RUN=$((RUN + 1))
  if ! dotnet test "$PROJ" -c Release --no-build \
        --filter "FullyQualifiedName=$FAILING_TEST" --nologo \
        > "/tmp/run-$RUN.log" 2>&1; then
    FAILS=$((FAILS + 1))
  fi
done
echo "Local reproduction: $FAILS / $RUN failures"
```

- `0/N` and ≥3 PRs showed it → still treat as a flake (the race may not trigger on your hardware). Prefer **Option B** (quarantine), subject to the rule against quarantining tests the originating PR introduced.
- `1–(N-1)/N` → classic non-determinism. Prefer **Option A** (determinism fix).
- `N/N` → not a flake, it's a hard failure. Emit `noop` and comment on the originating PR explaining that `pr-build-status` should have classified this as a real failure.

Before proceeding to Step 3, run `gh pr diff ${{ inputs.originating_pr }} -- '<test file path>'`. If the originating PR added or changed this test file, **stop**: emit `noop` and comment on the originating PR:
```
🤖 *LabelOps Flake — skipped.* This test was introduced or modified by this PR itself; I will not quarantine it. If it is actually flaky, please investigate the test as authored.
```

### Step 3 — Fix

**Option A — Determinism fix** (strongly preferred when you can identify the race):

- Common patterns in this repo:
  - Tests that compare `DateTime.Now` or `DateTime.UtcNow` instead of a fixed clock.
  - Tests that assume parallel-test ordering (e.g., shared temp directory, shared `FSharpChecker` cache).
  - Tests relying on network resources — restructure to use local mocks.
  - Tests with tight timing thresholds — loosen or replace with deterministic checks.
- Apply the minimal fix inside the test file. Re-run the 20-iteration loop. Require `0/20` failures.
- Open a PR (see Step 4) with title `[LabelOps Flake] Fix <short test name> determinism`.

**Option B — Quarantine** (when the fix is non-trivial or out of `tests/**` scope):

- Open a tracking issue first (via `create-issue`):
  - Title: `Flaky test: <fully qualified test name>`
  - Body: evidence table (PR numbers + build IDs + dates from the skill), local reproduction stats, likely root cause if known.
- Add a skip marker on the test, referencing the issue:
  - xUnit: `[<Fact(Skip = "Flaky, tracked in #NNN")>]` or `[<Theory(Skip = "Flaky, tracked in #NNN")>]`.
  - NUnit: `[<Test; Ignore("Flaky, tracked in #NNN")>]`.
- Re-run locally to confirm it's now skipped.
- Open a PR (Step 4) with title `[LabelOps Flake] Quarantine <short test name>`.

### Step 4 — Open the PR

Use `create-pull-request`. Body template:

```
## Flaky test: `<fully qualified test name>`

### Evidence (collected by `flaky-test-detector`)

| PR | Build | Date |
|---|---|---|
| #19820 | 1234567 | 2025-mm-dd |
| #19833 | 1234589 | 2025-mm-dd |
| #19891 | 1234612 | 2025-mm-dd |

### Local reproduction

- Hardware: `$(uname -srm)`
- Loop: 20 iterations via `dotnet test --filter FullyQualifiedName~<test> --no-build -c Release`
- Failures observed: `N / 20`

### Fix strategy

- [x] Determinism fix | [ ] Quarantine (with tracking issue #NNN)

### Why

<one or two sentences>

### Originating PR

Triggered by #<originating_pr>.

`NO_RELEASE_NOTES`: test-only change, no user-visible behavior.
```

### Step 5 — Comment on the originating PR

Via `add-comment` with `target: <originating_pr>`:

```
🤖 *LabelOps Flake — dispatched.* Opened #<new-pr> to address `<test>` — seen across <N> PRs. Re-run this PR's checks once #<new-pr> merges.
```

## Checklist before emitting safe outputs

- [ ] I re-ran `flaky-test-detector` and it confirmed ≥3 recent failures.
- [ ] I reproduced locally (or documented why I couldn't and chose quarantine).
- [ ] My changes touch **only** `tests/**` or `vsintegration/tests/**`.
- [ ] I did not alter production code.
- [ ] I opened at most one PR and one issue (Option B only).
- [ ] I commented on the originating PR with the new PR number.
