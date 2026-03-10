<#
.SYNOPSIS
    Detects flaky tests by scanning recent CI builds for recurring test failures.

.DESCRIPTION
    Queries the Azure DevOps builds API for recent failed F# PR builds, collects
    test failures from each, and identifies tests that fail across multiple distinct
    PRs — a strong signal of flakiness rather than a genuine regression.

    Uses Get-BuildErrors.ps1 from the pr-build-status skill for test failure extraction.

.PARAMETER MaxBuilds
    Maximum number of failed builds to scan. Default: 50.

.PARAMETER MinPRFailures
    Minimum number of distinct PRs a test must fail in to be flagged. Default: 3.

.PARAMETER DaysBack
    Only consider builds from within this many days. Default: 14.

.PARAMETER DefinitionId
    Azure DevOps pipeline definition ID for fsharp-ci. Default: 90.

.PARAMETER Org
    Azure DevOps organization. Default: 'dnceng-public'.

.PARAMETER Project
    Azure DevOps project. Default: 'public'.

.PARAMETER ScriptsDir
    Path to pr-build-status scripts directory.

.EXAMPLE
    ./Get-FlakyTests.ps1
    ./Get-FlakyTests.ps1 -MaxBuilds 100 -MinPRFailures 2 -DaysBack 7
#>

[CmdletBinding()]
param(
    [int]$MaxBuilds = 50,
    [int]$MinPRFailures = 3,
    [int]$DaysBack = 14,
    [int]$DefinitionId = 90,
    [string]$Org = "dnceng-public",
    [string]$Project = "public",
    [string]$Repo = "dotnet/fsharp",
    [string]$ScriptsDir = (Join-Path $PSScriptRoot ".." ".." "pr-build-status" "scripts")
)

$ErrorActionPreference = "Stop"

# Resolve helper scripts
$getBuildErrors = Join-Path (Resolve-Path $ScriptsDir) "Get-BuildErrors.ps1"
if (-not (Test-Path $getBuildErrors)) {
    Write-Error "Cannot find Get-BuildErrors.ps1 at: $getBuildErrors"
    exit 1
}

$minTime = (Get-Date).AddDays(-$DaysBack).ToString("yyyy-MM-ddTHH:mm:ssZ")

# Step 1: Query AzDo for recent failed PR builds
Write-Host "`n=== Step 1: Fetching failed F# PR builds (last $DaysBack days, up to $MaxBuilds) ===" -ForegroundColor Cyan

$buildsUrl = "https://dev.azure.com/$Org/$Project/_apis/build/builds?definitions=$DefinitionId&reasonFilter=pullRequest&resultFilter=failed&minTime=$minTime&`$top=$MaxBuilds&api-version=7.0"

try {
    $buildsResponse = Invoke-RestMethod -Uri $buildsUrl -Method Get -ContentType "application/json"
}
catch {
    Write-Error "Failed to query AzDo builds API: $_"
    exit 1
}

$builds = $buildsResponse.value
if (-not $builds -or $builds.Count -eq 0) {
    Write-Host "No failed builds found in the last $DaysBack days." -ForegroundColor Green
    exit 0
}
Write-Host "Found $($builds.Count) failed build(s)" -ForegroundColor Green

# Group all failed builds by PR number
$buildsByPR = @{}
foreach ($build in $builds) {
    $prNum = $build.triggerInfo.'pr.number'
    if (-not $prNum) { continue }

    if (-not $buildsByPR.ContainsKey($prNum)) {
        $buildsByPR[$prNum] = @()
    }
    $buildsByPR[$prNum] += $build
}

Write-Host "Across $($buildsByPR.Count) distinct PR(s)" -ForegroundColor Green

# Step 2: For each build, collect test failures
Write-Host "`n=== Step 2: Scanning builds for test failures ===" -ForegroundColor Cyan

# Track: TestName -> list of {PR, BuildId, Job, ErrorMessage}
$testFailures = @{}
$scannedBuilds = 0
$perBuildTimeoutSec = 120

foreach ($entry in $buildsByPR.GetEnumerator()) {
    $prNum = $entry.Key
    $prBuilds = $entry.Value

    # Scan all failed builds for this PR (not just latest — each run may fail different tests)
    foreach ($build in $prBuilds) {
        $buildId = $build.id
        $buildDate = if ($build.startTime -is [datetime]) { $build.startTime.ToString("yyyy-MM-dd") } else { $build.startTime.Substring(0, 10) }
        Write-Host "Build $buildId (PR #$prNum, $buildDate)..." -ForegroundColor White -NoNewline

        try {
            # Run with timeout to avoid hanging on large builds
            $job = Start-Job -ScriptBlock {
                param($script, $id, $org, $proj)
                & $script -BuildId $id -TestsOnly -Org $org -Project $proj 2>&1
            } -ArgumentList $getBuildErrors, $buildId, $Org, $Project

            $completed = Wait-Job $job -Timeout $perBuildTimeoutSec
            if (-not $completed) {
                Write-Host " [timeout after ${perBuildTimeoutSec}s, skipping]" -ForegroundColor DarkYellow
                Stop-Job $job
                Remove-Job $job -Force
                $scannedBuilds++
                continue
            }

            $errors = Receive-Job $job
            Remove-Job $job -Force

            $testResults = $errors | Where-Object { $_ -is [PSCustomObject] -and $_.Type -eq "TestFailure" }

            if (-not $testResults -or @($testResults).Count -eq 0) {
                Write-Host " [no test failures]" -ForegroundColor DarkGreen
            }
            else {
                Write-Host " [$(@($testResults).Count) test failure(s)]" -ForegroundColor Yellow

                foreach ($result in $testResults) {
                    $testName = $result.Message
                    if (-not $testName) { continue }

                    # Normalize: strip parameterized suffixes like (arg1, arg2)
                    $normalizedName = $testName -replace '\(.*\)$', ''
                    $normalizedName = $normalizedName.Trim()

                    if (-not $testFailures.ContainsKey($normalizedName)) {
                        $testFailures[$normalizedName] = @()
                    }

                    $testFailures[$normalizedName] += [PSCustomObject]@{
                        PR       = $prNum
                        BuildId  = $buildId
                        Job      = $result.Source
                        Error    = $result.Details
                        FullName = $testName
                        Date     = $buildDate
                    }
                }
            }
        }
        catch {
            Write-Warning " [error: $_]"
        }

        $scannedBuilds++
    }
}

# Step 3: Aggregate and filter
Write-Host "`n=== Step 3: Analyzing results ===" -ForegroundColor Cyan
Write-Host "Scanned $scannedBuilds builds across $($buildsByPR.Count) PRs" -ForegroundColor White
Write-Host "Found $($testFailures.Count) distinct test(s) that failed at least once" -ForegroundColor White

# Group by distinct PRs
$flakyTests = $testFailures.GetEnumerator() | ForEach-Object {
    $testName = $_.Key
    $failures = $_.Value
    $distinctPRs = ($failures | Select-Object -ExpandProperty PR -Unique)
    $distinctJobs = ($failures | Select-Object -ExpandProperty Job -Unique)

    [PSCustomObject]@{
        TestName      = $testName
        DistinctPRs   = $distinctPRs.Count
        PRNumbers     = ($distinctPRs | Sort-Object) -join ", "
        TotalFailures = $failures.Count
        Jobs          = ($distinctJobs | Sort-Object) -join "; "
        SampleError   = ($failures | Select-Object -First 1).Error
    }
} | Where-Object { $_.DistinctPRs -ge $MinPRFailures } | Sort-Object -Property DistinctPRs -Descending

# Step 4: Cross-reference with recent PRs that may fix flaky tests
Write-Host "`n=== Step 4: Cross-referencing with recent PRs ===" -ForegroundColor Cyan

$fixPRs = @{}  # TestName -> list of matching PRs

if ($flakyTests.Count -gt 0) {
    $savedPager = $env:GH_PAGER
    $env:GH_PAGER = ""

    # Batch 1: Get all recent PRs mentioning "flaky" in the repo
    $allCandidatePRs = @()
    foreach ($searchState in @("open", "closed")) {
        try {
            $json = gh search prs --repo $Repo --limit 30 --state $searchState --json number,title,state,createdAt "flaky" 2>$null
            if ($json) { $allCandidatePRs += ($json | ConvertFrom-Json) }
        }
        catch {}
    }

    # Batch 2: For each flaky test, search for PRs mentioning its class/method name
    foreach ($test in $flakyTests) {
        $parts = $test.TestName -split '\.'
        $searchTerms = @()
        if ($parts.Count -ge 2) {
            $searchTerms += $parts[-1]   # Method/test name
            $searchTerms += $parts[-2]   # Class name
        }
        else {
            $searchTerms += $test.TestName
        }

        foreach ($term in $searchTerms) {
            if ($term.Length -lt 5) { continue }
            try {
                $json = gh search prs --repo $Repo --limit 10 --json number,title,state,createdAt "$term" 2>$null
                if ($json) { $allCandidatePRs += ($json | ConvertFrom-Json) }
            }
            catch {}
        }
    }

    $env:GH_PAGER = $savedPager

    # Deduplicate
    $allCandidatePRs = $allCandidatePRs | Sort-Object -Property number -Unique

    # Common words to exclude from matching (too generic)
    $stopWords = @("test", "tests", "testing", "skip", "flaky", "core", "compiler", "fsharp",
                    "service", "component", "unit", "type", "failed", "error", "variant",
                    "conformance", "basic", "grammar", "elements", "custom", "attributes")

    # Match candidate PRs to flaky tests by specific keyword overlap
    foreach ($test in $flakyTests) {
        $testName = $test.TestName
        $matchingPRs = @()

        # Extract specific keywords (≥5 chars, not stop words)
        $keywords = ($testName -split '[\.\+\s_\-]') | Where-Object {
            $_.Length -ge 5 -and ($stopWords -notcontains $_.ToLower())
        }

        foreach ($pr in $allCandidatePRs) {
            $titleLower = $pr.title.ToLower()
            foreach ($kw in $keywords) {
                if ($titleLower -match [regex]::Escape($kw.ToLower())) {
                    $matchingPRs += $pr
                    break
                }
            }
        }

        if ($matchingPRs.Count -gt 0) {
            $fixPRs[$testName] = $matchingPRs
        }
    }

    Write-Host "Found $($fixPRs.Count) flaky test(s) with potentially related PRs" -ForegroundColor Green
}

# Step 5: Report
Write-Host "`n" -NoNewline
Write-Host "============================================" -ForegroundColor Magenta
Write-Host "  FLAKY TEST REPORT" -ForegroundColor Magenta
Write-Host "  Window: last $DaysBack days | Threshold: $MinPRFailures+ distinct PRs" -ForegroundColor Magenta
Write-Host "============================================" -ForegroundColor Magenta

if ($flakyTests.Count -eq 0) {
    Write-Host "`nNo flaky tests detected (threshold: $MinPRFailures distinct PRs)." -ForegroundColor Green
    Write-Host "Try lowering -MinPRFailures or increasing -DaysBack / -MaxBuilds." -ForegroundColor DarkGray
}
else {
    Write-Host "`nFound $($flakyTests.Count) flaky test(s):`n" -ForegroundColor Red

    $rank = 1
    foreach ($test in $flakyTests) {
        Write-Host "$rank. $($test.TestName)" -ForegroundColor Yellow
        Write-Host "   Failed in $($test.DistinctPRs) PRs (#$($test.PRNumbers))" -ForegroundColor White
        Write-Host "   Total failures: $($test.TotalFailures) | Jobs: $($test.Jobs)" -ForegroundColor DarkGray
        if ($test.SampleError) {
            $truncated = if ($test.SampleError.Length -gt 200) { $test.SampleError.Substring(0, 200) + "..." } else { $test.SampleError }
            Write-Host "   Sample error: $truncated" -ForegroundColor DarkGray
        }

        # Show related fix PRs
        if ($fixPRs.ContainsKey($test.TestName)) {
            $relatedPRs = $fixPRs[$test.TestName]
            foreach ($pr in $relatedPRs) {
                $status = if ($pr.state -eq "merged") { "MERGED" } elseif ($pr.state -eq "open") { "OPEN" } else { $pr.state.ToUpper() }
                $color = if ($status -eq "MERGED") { "Green" } elseif ($status -eq "OPEN") { "Cyan" } else { "DarkGray" }
                Write-Host "   >> Related PR #$($pr.number) [$status]: $($pr.title)" -ForegroundColor $color
            }
        }
        else {
            Write-Host "   >> No related fix PRs found" -ForegroundColor DarkRed
        }

        Write-Host ""
        $rank++
    }
}

# Add RelatedPRs property to structured output
$flakyTests | ForEach-Object {
    $testName = $_.TestName
    $related = if ($fixPRs.ContainsKey($testName)) {
        $fixPRs[$testName] | ForEach-Object { "PR #$($_.number) [$($_.state)]: $($_.title)" }
    }
    else { @() }
    $_ | Add-Member -NotePropertyName "RelatedPRs" -NotePropertyValue ($related -join "; ") -PassThru
}
