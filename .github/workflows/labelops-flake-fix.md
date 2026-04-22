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
  web-fetch:
  github:
    toolsets: [all]
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
    max: 3
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
5. **Always prefix comments with `🤖 *LabelOps Flake — <subtopic>.*`**.

## Process

### Step 1 — Re-verify the flake

Run the `flaky-test-detector` skill with `failing_test`. Require evidence across ≥3 of the `affected_prs`. If the skill no longer finds evidence (the test may have been fixed on `main` since dispatch), post on the originating PR:
```
🤖 *LabelOps Flake — not reproducible.* Re-check of `<test>` found no recent failures. No action taken.
```
Emit `noop` and exit.

### Step 2 — Reproduce locally

```bash
# Determine the containing test project from the fully qualified name.
# Typical projects: tests/FSharp.Compiler.ComponentTests, tests/FSharp.Core.UnitTests,
#                   tests/FSharp.Compiler.Service.Tests, vsintegration/tests/...
PROJ=$(grep -rl "$(echo $FAILING_TEST | awk -F. '{print $(NF-1)"."$NF}')" tests/ vsintegration/tests/ \
       --include='*.fs' | head -1 | xargs -I{} dirname {} | xargs -I{} \
       find {} -maxdepth 4 -name '*.fsproj' | head -1)

# Run the test 20 times; count failures.
FAILS=0
for i in $(seq 1 20); do
  if ! dotnet test "$PROJ" -c Release --no-build --filter "FullyQualifiedName~$FAILING_TEST" --nologo -- --report-xml-filename "run-$i.xml" >/dev/null 2>&1; then
    FAILS=$((FAILS + 1))
  fi
done
echo "Local reproduction: $FAILS / 20 failures"
```

- `0/20` and ≥3 PRs showed it → still treat as a flake (the race may not trigger on your hardware). Prefer **Option B** (quarantine).
- `1–19/20` → classic non-determinism. Prefer **Option A** (determinism fix).
- `20/20` → not a flake, it's a hard failure. Emit `noop` and comment on the originating PR explaining that `pr-build-status` should have classified this as a real failure.

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
