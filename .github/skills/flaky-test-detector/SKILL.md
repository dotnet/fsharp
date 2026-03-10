---
name: flaky-test-detector
description: "Detect flaky tests by scanning recent CI builds across multiple PRs. Use when investigating intermittent test failures, CI instability, or deciding which tests to quarantine."
metadata:
  author: fsharp-team
  version: "1.0"
---

# Flaky Test Detector

Identifies tests that fail intermittently across unrelated PRs — a strong signal of flakiness rather than a genuine regression.

## When to Use

- Investigating CI instability ("is this test failure my fault or flaky?")
- Periodic hygiene: finding tests to quarantine or fix
- Before marking a test as `Skip = "Flaky"` — confirm it actually is flaky

## How It Works

The detector scans recent PRs, collects test failures from their Azure DevOps CI builds, and cross-references failures by test name. A test failing in **3+ distinct PRs** within a time window is flagged as flaky.

## Usage

### Quick scan (last 14 days, 50 builds, threshold = 3)

```bash
pwsh .github/skills/flaky-test-detector/scripts/Get-FlakyTests.ps1
```

### Custom parameters

```bash
# More aggressive: 2+ PRs over 7 days
pwsh .github/skills/flaky-test-detector/scripts/Get-FlakyTests.ps1 -MinPRFailures 2 -DaysBack 7

# Wider net: 100 builds over 30 days  
pwsh .github/skills/flaky-test-detector/scripts/Get-FlakyTests.ps1 -MaxBuilds 100 -DaysBack 30
```

### Parameters

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-MaxBuilds` | 50 | Maximum number of failed builds to scan from AzDo |
| `-MinPRFailures` | 3 | Min distinct PRs a test must fail in to be flagged |
| `-DaysBack` | 14 | Only consider builds within this time window |
| `-DefinitionId` | 90 | AzDo pipeline definition ID (90 = fsharp-ci) |
| `-Org` | dnceng-public | Azure DevOps organization |
| `-Project` | public | Azure DevOps project |

## Output

The script produces:
1. **Console report** with ranked flaky tests, PR numbers, job names, and sample errors
2. **Structured objects** (PowerShell) for programmatic consumption

## Interpreting Results

- **DistinctPRs ≥ 5**: Almost certainly flaky — consider quarantining immediately
- **DistinctPRs = 3–4**: Likely flaky — investigate root cause
- **DistinctPRs = 2**: Possibly flaky or a shared dependency issue — monitor

## Follow-up Actions

After identifying a flaky test:
1. Check if there's already a GitHub issue for it
2. If not, file one with the `Area-flaky-test` label
3. Consider marking with `[<Fact(Skip = "Flaky: #ISSUE")>]` if it blocks CI
4. Fix the root cause (timing, file locking, thread safety, etc.)
