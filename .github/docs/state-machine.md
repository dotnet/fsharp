# Agentic Workflow State Machine

Auto-generated documentation of all agentic workflows in this repository.

## Workflow Overview

| Workflow | Trigger | Reads | Writes | Key Labels |
|----------|---------|-------|--------|------------|
| **repo-assist** | ⏰ every 12h, `/repo-assist` | Issues, PRs, code, tests | comment, PR, issue, labels | `AI-thinks-issue-fixed`, `AI-thinks-windows-only`, `AI-Issue-Regression-PR` |
| **labelops-pr-maintenance** | ⏰ every 3h | PRs with AI-Auto-Resolve-* labels, CI status | comment, push, labels, dispatch | `AI-Auto-Resolve-CI`, `AI-Auto-Resolve-Conflicts`, `AI-needs-CI-fix-input` |
| **regression-pr-shepherd** | ⏰ every 4h | PRs with `AI-Issue-Regression-PR` | comment, push, remove-labels | `AI-Issue-Regression-PR`, `AI-thinks-issue-fixed` |
| **labelops-flake-fix** | 🤖 dispatched by labelops-pr-maintenance | Test results, PR diffs | PR, comment, issue | `Flaky`, `automation` |
| **aw-auto-update** | ⏰ every 24h | `.github/workflows/*` files | PR, push | `automation` |

## Issue Lifecycle

```mermaid
stateDiagram-v2
    direction LR

    [*] --> Open: 👤 contributor files issue
    Open --> Investigated: 🤖 repo-assist (⏰ 12h)
    Investigated --> FixedCandidate: 🤖 repo-assist adds AI-thinks-issue-fixed
    Investigated --> WindowsOnly: 🤖 repo-assist adds AI-thinks-windows-only

    state "Fix Verification" as FixVerify {
        FixedCandidate --> TestExists: 🤖 repo-assist finds existing test
        FixedCandidate --> TestPRCreated: 🤖 repo-assist creates regression test PR
        FixedCandidate --> NotActuallyFixed: 🤖 repo-assist/shepherd removes label
    }

    TestExists --> Closed: 👤 maintainer closes
    TestPRCreated --> Closed: ⚙️ CI passes + 👤 maintainer merges PR
    NotActuallyFixed --> Open: label removed, issue remains open

    state "Windows-Only Reassessment" as WinReassess {
        WindowsOnly --> Reclassified: 🤖 repo-assist removes AI-thinks-windows-only
        WindowsOnly --> ConfirmedWinOnly: 🤖 repo-assist keeps label
    }

    Reclassified --> Investigated: 🤖 repo-assist re-investigates
```

## Regression Test PR Lifecycle

```mermaid
stateDiagram-v2
    direction LR

    [*] --> Created: 🤖 repo-assist creates PR (⏰ 12h)

    state "Shepherd Loop (⏰ 4h)" as ShepherdLoop {
        Created --> Categorize: 🤖 regression-pr-shepherd

        state categorize <<choice>>
        Categorize --> categorize
        categorize --> HasFeedback: review comments exist
        categorize --> CIFailing: checks failed
        categorize --> Healthy: all green

        HasFeedback --> FixPushed: 🤖 shepherd addresses feedback
        CIFailing --> InfraRetry: 🤖 shepherd retries (flaky/infra)
        CIFailing --> CompileFixed: 🤖 shepherd fixes test code
        CIFailing --> BugStillExists: 🤖 shepherd detects real failure
        CIFailing --> MergeResolved: 🤖 shepherd rebases

        FixPushed --> Created: ⚙️ CI restarts
        InfraRetry --> Created: ⚙️ CI restarts
        CompileFixed --> Created: ⚙️ CI restarts
        MergeResolved --> Created: ⚙️ CI restarts
    }

    Healthy --> Merged: 👤 maintainer merges
    BugStillExists --> Closed: 🤖 shepherd closes PR + removes AI-thinks-issue-fixed

    Merged --> [*]
    Closed --> [*]
```

## PR Maintenance Lifecycle

```mermaid
stateDiagram-v2
    direction LR

    [*] --> Labelled: 👤 maintainer adds AI-Auto-Resolve-* label

    state "Maintenance Loop (⏰ 3h)" as MaintLoop {
        Labelled --> ClassifyPR: 🤖 labelops-pr-maintenance

        state classify <<choice>>
        ClassifyPR --> classify
        classify --> CICheck: has AI-Auto-Resolve-CI
        classify --> ConflictCheck: has AI-Auto-Resolve-Conflicts

        CICheck --> CIHealthy: ⚙️ all checks pass
        CICheck --> CIFixable: 🤖 labelops fixes CI
        CICheck --> ProvenFlake: 🤖 labelops detects flake
        CICheck --> Escalated: 🤖 labelops adds AI-needs-CI-fix-input

        CIFixable --> Labelled: ⚙️ CI restarts after push
        ProvenFlake --> FlakeDispatched: 🤖 labelops dispatches labelops-flake-fix
        Escalated --> Blocked: 👤 maintainer needed

        Blocked --> Labelled: 👤 maintainer pushes fix (unblocks)

        CIHealthy --> ConflictCheck
        ConflictCheck --> NoConflicts: merge-tree clean
        ConflictCheck --> ConflictResolved: 🤖 labelops resolves conflicts
        ConflictCheck --> ConflictFailed: 🤖 labelops cannot resolve

        ConflictResolved --> Labelled: ⚙️ CI restarts after push
        ConflictFailed --> Labelled: comment posted, no push
    }

    NoConflicts --> Ready: PR is mergeable
    Ready --> Merged: 👤 maintainer merges

    state "Flake Fix Spinoff" as FlakeFix {
        FlakeDispatched --> FlakeVerified: 🤖 labelops-flake-fix re-verifies
        FlakeVerified --> FlakeFixPR: 🤖 flake-fix opens fix/quarantine PR
    }

    Merged --> [*]
```

## Infrastructure Lifecycle

```mermaid
stateDiagram-v2
    direction LR

    [*] --> CheckUpdate: ⏰ aw-auto-update (24h)

    state check <<choice>>
    CheckUpdate --> check
    check --> UpToDate: no changes detected
    check --> ChangesDetected: gh aw upgrade produced diff

    UpToDate --> [*]: 🤖 aw-auto-update noops

    ChangesDetected --> PRExists: 🤖 checks for existing PR

    state prcheck <<choice>>
    PRExists --> prcheck
    prcheck --> UpdateExisting: open PR found
    prcheck --> CreateNew: no open PR

    UpdateExisting --> WaitReview: 🤖 aw-auto-update pushes to branch
    CreateNew --> WaitReview: 🤖 aw-auto-update creates PR

    WaitReview --> Merged: 👤 maintainer reviews + merges
    Merged --> [*]
```

## Label Dictionary

| Label | Applied By | Read By | Meaning |
|-------|-----------|---------|---------|
| `AI-thinks-issue-fixed` | 🤖 repo-assist | 🤖 repo-assist, 🤖 regression-pr-shepherd | Issue appears fixed; needs regression test verification |
| `AI-thinks-windows-only` | 🤖 repo-assist | 🤖 repo-assist | Issue requires Windows/VS to reproduce (may be reassessed) |
| `AI-Auto-Resolve-CI` | 👤 maintainer | 🤖 labelops-pr-maintenance | Opt-in: agent should fix CI failures on this PR |
| `AI-Auto-Resolve-Conflicts` | 👤 maintainer | 🤖 labelops-pr-maintenance | Opt-in: agent should resolve merge conflicts on this PR |
| `AI-needs-CI-fix-input` | 🤖 labelops-pr-maintenance | 🤖 labelops-pr-maintenance, 👤 maintainer | CI failure requires human intervention |
| `AI-Issue-Regression-PR` | 🤖 repo-assist | 🤖 regression-pr-shepherd, 🤖 labelops-pr-maintenance (exclude) | PR is a regression test created by repo-assist |
| `Flaky` | 🤖 labelops-flake-fix | 👤 maintainer | Test identified as non-deterministic |
| `automation` | 🤖 aw-auto-update, 🤖 labelops-flake-fix | 👤 maintainer | PR was created by automation |
| `NO_RELEASE_NOTES` | 🤖 repo-assist, 🤖 labelops-flake-fix | ⚙️ CI | PR does not need release notes entry |
| `repo-assist` | 🤖 repo-assist | 🤖 repo-assist | Issue is managed by repo-assist (monthly summary) |

## Handover Map

| From | To | Trigger | Mechanism |
|------|----|---------|-----------|
| 🤖 repo-assist | 🤖 regression-pr-shepherd | PR created with `AI-Issue-Regression-PR` label | Label-based pickup (⏰ 4h) |
| 🤖 labelops-pr-maintenance | 🤖 labelops-flake-fix | Proven flake detected (≥3 PRs) | `dispatch-workflow` |
| 🤖 regression-pr-shepherd | 👤 maintainer | PR is healthy (CI green, no feedback) | PR ready for review |
| 🤖 regression-pr-shepherd | 👤 maintainer | Bug still exists (Category B3) | Comment + close PR + remove label |
| 🤖 labelops-pr-maintenance | 👤 maintainer | CI unfixable | `AI-needs-CI-fix-input` label + escalation comment |
| 👤 maintainer | 🤖 labelops-pr-maintenance | Adds `AI-Auto-Resolve-*` label to PR | Label-based pickup (⏰ 3h) |
| 👤 maintainer | 🤖 repo-assist | `/repo-assist <instructions>` | Slash command |
| ⏰ scheduler | 🤖 repo-assist | Every 12h | Cron schedule |
| ⏰ scheduler | 🤖 labelops-pr-maintenance | Every 3h | Cron schedule |
| ⏰ scheduler | 🤖 regression-pr-shepherd | Every 4h | Cron schedule |
| ⏰ scheduler | 🤖 aw-auto-update | Every 24h | Cron schedule |
| 🤖 repo-assist | 🤖 repo-assist | Own PR has CI failure or conflicts | `push-to-pull-request-branch` (self-heal) |
| 🤖 labelops-flake-fix | 🤖 labelops-pr-maintenance | Fix PR created | Originating PR comment posted |

<!-- source-hashes:
aw-auto-update.md: c6643ba35ba34b092b3d8f8bcf09f5310145a4ab64df29b18b47aa4c562fae2a
labelops-flake-fix.md: 7dca5b8faa60f947204f8925c6238fbecf42aa8cbf3144a166120501b0eef1e4
labelops-pr-maintenance.md: 0bc03e9762ba51dc4226e0fc9aeb8fd9ef8fb17b287ae48fd67e2d33beb92473
regression-pr-shepherd.md: 18a65fe1cdf8aa219158f1d610db14078e5ff2f1ac912df2566bf796792395b5
repo-assist.md: ee557b9645115a600035c441e476e030e5a67aa1bca93dfd94985bf6f12e1d74
-->
