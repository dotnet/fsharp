---
name: pr-build-status
description: "Retrieve and analyze Azure DevOps build failures for GitHub PRs. Use when CI fails. CRITICAL: Collect ALL errors from ALL platforms FIRST, write hypotheses to file, then fix systematically."
metadata:
  author: dotnet-maui
  version: "2.0"
compatibility: Requires GitHub CLI (gh) authenticated with access to dotnet/fsharp repository.
---

# PR Build Status Skill

Retrieve and systematically analyze Azure DevOps build failures for GitHub PRs.

## CRITICAL: Collect-First Workflow

**DO NOT push fixes until ALL errors are collected and reproduced locally.**

LLMs tend to focus on the first error found and ignore others. This causes:
- Multiple push/wait/fail cycles
- CI results being overwritten before full analysis
- Missing platform-specific failures (Linux vs Windows vs MacOS)

### Mandatory Workflow

```
1. COLLECT ALL     → Get errors from ALL jobs across ALL platforms
2. DOCUMENT        → Write CI_ERRORS.md with hypotheses per platform
3. REPRODUCE       → Run each failing test LOCALLY (in isolation!)
4. FIX             → Fix each issue, verify locally
5. PUSH            → Only after ALL issues verified fixed
```

## Scripts

All scripts are in `.github/skills/pr-build-status/scripts/`

### 1. Get Build IDs for a PR
```powershell
pwsh .github/skills/pr-build-status/scripts/Get-PrBuildIds.ps1 -PrNumber <PR_NUMBER>
```

### 2. Get Build Status (List ALL Failed Jobs)
```powershell
# Get overview of all stages and jobs
pwsh .github/skills/pr-build-status/scripts/Get-BuildInfo.ps1 -BuildId <BUILD_ID>

# Get ONLY failed jobs (use this to see all failing platforms)
pwsh .github/skills/pr-build-status/scripts/Get-BuildInfo.ps1 -BuildId <BUILD_ID> -FailedOnly
```

### 3. Get Build Errors and Test Failures
```powershell
# Get ALL errors (build errors + test failures) - USE THIS FIRST
pwsh .github/skills/pr-build-status/scripts/Get-BuildErrors.ps1 -BuildId <BUILD_ID>

# Filter to specific job (after getting overview)
pwsh .github/skills/pr-build-status/scripts/Get-BuildErrors.ps1 -BuildId <BUILD_ID> -JobFilter "*Linux*"
pwsh .github/skills/pr-build-status/scripts/Get-BuildErrors.ps1 -BuildId <BUILD_ID> -JobFilter "*Windows*"
pwsh .github/skills/pr-build-status/scripts/Get-BuildErrors.ps1 -BuildId <BUILD_ID> -JobFilter "*MacOS*"
```

### 4. Direct API Access (for detailed logs)
```powershell
# Get timeline with all jobs
$uri = "https://dev.azure.com/dnceng-public/public/_apis/build/builds/<BUILD_ID>/timeline?api-version=7.1"
Invoke-RestMethod -Uri $uri | Select-Object -ExpandProperty records | Where-Object { $_.result -eq "failed" }

# Get specific log content
$logUri = "https://dev.azure.com/dnceng-public/cbb18261-c48f-4abb-8651-8cdcb5474649/_apis/build/builds/<BUILD_ID>/logs/<LOG_ID>"
Invoke-RestMethod -Uri $logUri | Select-String "Failed|Error|FAIL"
```

## Step-by-Step Analysis Procedure

### Step 1: Get Failed Build ID
```powershell
pwsh .github/skills/pr-build-status/scripts/Get-PrBuildIds.ps1 -PrNumber XXXXX
# Note the BuildId with FAILED state
```

### Step 2: List ALL Failed Jobs (Cross-Platform!)
```powershell
pwsh .github/skills/pr-build-status/scripts/Get-BuildInfo.ps1 -BuildId YYYYY -FailedOnly
```
**IMPORTANT**: Note jobs from EACH platform:
- Linux jobs
- Windows jobs  
- MacOS jobs
- Different test configurations (net10.0 vs net472, etc.)

### Step 3: Get Errors Per Platform
```powershell
# Collect errors from EACH platform separately
pwsh .github/skills/pr-build-status/scripts/Get-BuildErrors.ps1 -BuildId YYYYY -JobFilter "*Linux*"
pwsh .github/skills/pr-build-status/scripts/Get-BuildErrors.ps1 -BuildId YYYYY -JobFilter "*Windows*"
pwsh .github/skills/pr-build-status/scripts/Get-BuildErrors.ps1 -BuildId YYYYY -JobFilter "*MacOS*"
```

### Step 4: Write CI_ERRORS.md
Create a file in session workspace with ALL findings:
```markdown
# CI Errors for PR #XXXXX - Build YYYYY

## Failed Jobs Summary
| Platform | Job Name | Error Type |
|----------|----------|------------|
| Linux    | ...      | Test       |
| Windows  | ...      | Test       |

## Hypothesis Per Platform

### Linux/MacOS Failures
- Error: "The type 'int' is not defined"
- Hypothesis: Missing FSharp.Core reference in test setup
- Reproduction: `dotnet test ... -f net10.0`

### Windows Failures  
- Error: "Expected cache hits for generic patterns"
- Hypothesis: Flaky test assertion, passes with other tests
- Reproduction: `dotnet test ... --filter "FullyQualifiedName~rigid generic"`

## Reproduction Commands
...

## Fix Verification Checklist
- [ ] Linux error reproduced locally
- [ ] Windows error reproduced locally
- [ ] Fix verified for Linux
- [ ] Fix verified for Windows
- [ ] Tests run IN ISOLATION (not just with other tests)
```

### Step 5: Reproduce Locally BEFORE Fixing
```powershell
# Run failing tests IN ISOLATION (critical!)
dotnet test ... --filter "FullyQualifiedName~FailingTestName" -f net10.0

# Run multiple times to check for flakiness
for ($i = 1; $i -le 3; $i++) { dotnet test ... }
```

### Step 6: Fix and Verify
Only after ALL issues reproduced:
1. Fix each issue
2. Verify each fix locally (run test in isolation!)
3. Run full test suite
4. Check formatting
5. THEN push

## Common Pitfalls

### ❌ Mistake: Focus on First Error Only
```
See Linux error → Fix → Push → Wait → See Windows error → Fix → Push → ...
```

### ✅ Correct: Collect All First
```
See Linux error → See Windows error → See MacOS error → Document all → 
Fix all → Verify all locally → Push once
```

### ❌ Mistake: Run Tests Together
```
dotnet test ... --filter "OverloadCacheTests"  # All 8 pass together
```

### ✅ Correct: Run Tests in Isolation
```
dotnet test ... --filter "FullyQualifiedName~specific test name"  # May fail alone!
```

## Prerequisites

- `gh` (GitHub CLI) - authenticated
- `pwsh` (PowerShell 7+)
- Local build environment matching CI