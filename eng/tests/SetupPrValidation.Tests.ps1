$ErrorActionPreference = 'Stop'
$setup = Join-Path $PSScriptRoot '..\setup-pr-validation.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ("fsharp-pr-validation-" + [Guid]::NewGuid())
$git = (Get-Command git -CommandType Application | Select-Object -First 1).Source
$previousGlobalConfig = $env:GIT_CONFIG_GLOBAL
$previousSystemConfig = $env:GIT_CONFIG_NOSYSTEM

function Invoke-FixtureGit {
    $output = & $git @args
    if ($LASTEXITCODE -ne 0) { throw "Fixture git command failed: $args" }
    $output
}

function Invoke-RestMethod {
    param($Uri, $Headers)
    if ($Uri -eq 'https://api.github.com/repos/dotnet/fsharp/pulls/1') {
        $prLookups.Add($Uri)
        if ($case -eq 'api-failure' -or ($case -like 'final-api-failure*' -and $prLookups.Count -eq 2)) {
            throw 'GitHub unavailable'
        }
        return $pr
    }
    if ($Uri -match '^https://api.github.com/repos/dotnet/fsharp/pulls/1/commits\?per_page=100&page=([1-3])$') {
        if ($case -eq 'commits-api-failure') { throw 'GitHub commits unavailable' }
        if ($case -eq 'retarget-during-commit-list') { $pr.base.ref = 'release/test' }
        if ($case -eq 'close-during-commit-list') { $pr.state = 'closed' }
        $page = [int]$Matches[1]
        $requiredPage = switch ($case) { 'commit-on-page-two' { 2 }; 'commit-on-page-three' { 3 }; default { 1 } }
        $entries = @(if ($page -lt $requiredPage) {
            @(1..100 | ForEach-Object { [pscustomobject]@{ sha = $base } })
        }
        elseif ($case -eq 'force-pushed-away') { @([pscustomobject]@{ sha = $later }) }
        elseif ($case -eq 'invalid-commit-list') { @([pscustomobject]@{ sha = 'invalid' }) }
        else { @([pscustomobject]@{ sha = $head }, [pscustomobject]@{ sha = $later }) })
        Write-Output -NoEnumerate $entries
        return
    }
    throw "Unexpected URL: $Uri"
}

function git {
    $ErrorActionPreference = 'Continue'
    if ($args[0] -eq 'checkout') { $checkoutCalls.Add(($args -join ' ')) }
    if ($args[0] -eq 'fetch' -and $args[-1] -eq 'refs/pull/1/merge' -and $case -eq 'head-and-merge-changed-during-fetch') {
        Invoke-FixtureGit -C $remote update-ref refs/pull/1/head $later
        Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $newHeadMerge
    }
    if ($args[0] -eq 'fetch' -and $args[-1] -eq 'refs/pull/1/merge' -and $case -eq 'retarget-during-fetch') {
        $pr.base.ref = 'release/test'
        Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $newBaseMerge
    }
    $output = & $git @args 2>&1
    $exitCode = $LASTEXITCODE
    if ($args[0] -eq 'fetch' -and $args[-1] -eq 'refs/pull/1/merge') {
        if ($case -eq 'head-changed-during-fetch') {
            Invoke-FixtureGit -C $remote update-ref refs/pull/1/head $later
        }
        if ($case -eq 'main-changed-during-fetch') {
            Invoke-FixtureGit -C $remote update-ref refs/heads/main $later
        }
        if ($case -eq 'close-during-fetch') { $pr.state = 'closed' }
        if ($case -eq 'repository-changed-during-fetch') { $pr.base.repo.full_name = 'other/fsharp' }
    }
    $global:LASTEXITCODE = $exitCode
    $output | ForEach-Object { "$_" }
}

New-Item -ItemType Directory $root | Out-Null
Push-Location $root
try {
    $env:GIT_CONFIG_GLOBAL = Join-Path $root 'gitconfig'
    $env:GIT_CONFIG_NOSYSTEM = '1'
    Invoke-FixtureGit init -q -b main remote
    Set-Location remote
    Invoke-FixtureGit config user.name Test
    Invoke-FixtureGit config user.email test@example.invalid
    Invoke-FixtureGit commit -q --allow-empty -m base
    $base = Invoke-FixtureGit rev-parse HEAD
    Invoke-FixtureGit checkout -q -b feature
    Invoke-FixtureGit commit -q --allow-empty -m head
    $head = Invoke-FixtureGit rev-parse HEAD
    Invoke-FixtureGit checkout -q main
    Invoke-FixtureGit merge -q --no-ff feature -m merge
    $merge = Invoke-FixtureGit rev-parse HEAD
    Invoke-FixtureGit commit -q --allow-empty -m later
    $later = Invoke-FixtureGit rev-parse HEAD
    $tree = Invoke-FixtureGit rev-parse 'HEAD^{tree}'
    $newBaseMerge = Invoke-FixtureGit commit-tree $tree -p $later -p $head -m 'merge with newer base'
    $newHeadMerge = Invoke-FixtureGit commit-tree $tree -p $base -p $later -m 'merge with newer head'
    $remote = (Get-Location).Path.Replace('\', '/')
    Set-Location $root

    $cases = [ordered]@{
        valid = $null
        'head-ref-changed' = $null
        'base-ref-changed' = $null
        'merge-new-base' = $null
        'older-commit' = $null
        'older-missing-merge' = $null
        'commit-on-page-two' = $null
        'commit-on-page-three' = $null
        'head-changed-during-fetch' = $null
        'head-and-merge-changed-during-fetch' = $null
        'main-changed-during-fetch' = $null
        'retarget-during-fetch' = 'no longer targets'
        'close-during-fetch' = 'PR is closed'
        'retarget-during-commit-list' = 'no longer targets'
        'close-during-commit-list' = 'PR is closed'
        'final-api-failure' = 'GitHub unavailable'
        'final-api-failure-direct' = 'GitHub unavailable'
        'repository-changed-during-fetch' = 'no longer targets'
        'force-pushed-away' = 'not found in the PR commit list'
        'commits-api-failure' = 'GitHub commits unavailable'
        'invalid-commit-list' = 'invalid PR commit SHA'
        'merge-mismatch' = 'fetched PR merge'
        'stale-merge' = 'fetched PR merge'
        'missing-merge' = 'git fetch'
        'missing-head' = 'valid PR head ref'
        closed = 'PR is closed'
        'non-main' = 'PR is closed'
        'uppercase-main' = 'no longer targets'
        'foreign-target' = 'PR is closed'
        'api-head-changed' = $null
        'api-base-changed' = $null
        'api-failure' = 'GitHub unavailable'
        'wrong-reviewed-checkout' = 'reviewed PR checkout'
        'invalid-input' = 'headSha'
    }
    foreach ($case in $cases.Keys) {
        Invoke-FixtureGit -C $remote update-ref refs/heads/main $base
        Invoke-FixtureGit -C $remote update-ref refs/pull/1/head $head
        Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $merge
        Invoke-FixtureGit clone -q --no-local --depth=1 $remote $case
        Push-Location $case
        try {
            Invoke-FixtureGit config "url.$remote.insteadOf" 'https://github.com/dotnet/fsharp.git'
            Invoke-FixtureGit fetch -q --depth=1 $remote refs/pull/1/head
            Invoke-FixtureGit checkout -q --detach $head
            $pr = @{
                state = 'open'
                head = @{ sha = $head }
                base = @{ sha = $base; ref = 'main'; repo = @{ full_name = 'dotnet/fsharp' } }
            }
            $expectedHead = $head
            $expectedTested = $merge
            $expectedMode = 'merge'
            $prLookups = [Collections.Generic.List[string]]::new()
            $checkoutCalls = [Collections.Generic.List[string]]::new()
            switch ($case) {
                { $_ -in 'head-ref-changed', 'older-commit', 'older-missing-merge', 'commit-on-page-two', 'commit-on-page-three', 'force-pushed-away', 'commits-api-failure', 'invalid-commit-list', 'retarget-during-commit-list', 'close-during-commit-list', 'final-api-failure-direct' } {
                    Invoke-FixtureGit -C $remote update-ref refs/pull/1/head $later
                    $expectedTested = $head
                    $expectedMode = 'commit'
                    if ($case -ne 'head-ref-changed') { $pr.head.sha = $later }
                    if ($case -eq 'older-missing-merge') { Invoke-FixtureGit -C $remote update-ref -d refs/pull/1/merge }
                }
                { $_ -in 'head-changed-during-fetch', 'head-and-merge-changed-during-fetch' } {
                    $expectedTested = $head
                    $expectedMode = 'commit'
                }
                'base-ref-changed' { Invoke-FixtureGit -C $remote update-ref refs/heads/main $later }
                'merge-new-base' {
                    Invoke-FixtureGit -C $remote update-ref refs/heads/main $later
                    Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $newBaseMerge
                    $expectedTested = $newBaseMerge
                }
                'merge-mismatch' { Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $later }
                'stale-merge' { Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $newHeadMerge }
                'missing-merge' { Invoke-FixtureGit -C $remote update-ref -d refs/pull/1/merge }
                'missing-head' { Invoke-FixtureGit -C $remote update-ref -d refs/pull/1/head }
                'closed' { $pr.state = 'closed' }
                'non-main' { $pr.base.ref = 'release/test' }
                'uppercase-main' { $pr.base.ref = 'Main' }
                'foreign-target' { $pr.base.repo.full_name = 'other/fsharp' }
                'api-head-changed' { $pr.head.sha = $later }
                'api-base-changed' { $pr.base.sha = $later }
                'wrong-reviewed-checkout' {
                    Invoke-FixtureGit fetch -q --depth=1 $remote refs/heads/main
                    Invoke-FixtureGit checkout -q --detach $base
                }
                'invalid-input' { $expectedHead = 'not-a-sha' }
            }
            $before = Invoke-FixtureGit rev-parse HEAD
            $failure = $null
            $logs = [Collections.Generic.List[string]]::new()
            try {
                & $setup -prNumber 1 -headSha $expectedHead 6>&1 |
                    ForEach-Object { $logs.Add("$_") }
            }
            catch { $failure = "$_" }
            if ($null -eq $cases[$case]) {
                if ($failure) { throw "$case failed: $failure" }
                if ((Invoke-FixtureGit rev-parse HEAD) -ne $expectedTested) { throw "Did not check out the expected $expectedMode commit" }
                if ($prLookups.Count -ne 2 -or $checkoutCalls.Count -ne 1) { throw 'Expected validation before selection and before checkout' }
                if ($logs -cnotcontains "PR #1: reviewed=$head mode=$expectedMode tested=$expectedTested") { throw 'Missing tested mode and commit diagnostic' }
                if ($logs -cnotcontains "##vso[task.setvariable variable=FSharp.TestedCommitSha]$expectedTested") { throw 'Missing tested commit variable' }
                if ((& $git config --get-regexp 'http\..*extraheader')) { throw 'Persisted checkout credentials' }
            }
            else {
                if (!$failure -or $failure -notmatch $cases[$case]) { throw "$case failed for wrong reason: $failure" }
                if ((Invoke-FixtureGit rev-parse HEAD) -ne $before) { throw "$case changed HEAD before validation" }
                if ($checkoutCalls.Count -ne 0) { throw "$case attempted checkout before validation completed" }
            }
            if ($case -eq 'retarget-during-fetch') {
                if ((Invoke-FixtureGit -C $remote rev-parse refs/heads/main) -ne $base -or
                    (Invoke-FixtureGit -C $remote rev-parse refs/pull/1/head) -ne $head) {
                    throw 'Retarget regression must keep main and PR head unchanged'
                }
            }
            if ($case -in 'valid', 'head-ref-changed', 'base-ref-changed', 'merge-new-base') {
                foreach ($expectedLog in "Requested snapshot: PR #1 reviewed=$head", "Initial checkout: $head", "GitHub PR: state=open target=dotnet/fsharp:main head=$head base=$base") {
                    if ($logs -cnotcontains $expectedLog) { throw "Missing diagnostic: $expectedLog" }
                }
                $actualHead = if ($case -eq 'head-ref-changed') { $later } else { $head }
                if ($logs -cnotcontains "  ${actualHead}<TAB>refs/pull/1/head") { throw 'Missing advertised PR head' }
                if ($case -ne 'head-ref-changed') {
                    $actualBase = if ($case -eq 'merge-new-base') { $later } else { $base }
                    if ($logs -cnotcontains "Fetched merge: $expectedTested parents=$actualBase $head") { throw 'Missing merge parent diagnostic' }
                }
                if ($logs | Where-Object { $_ -match '<TAB>refs/heads/main' }) {
                    throw 'Setup must not compare against moving main'
                }
            }
            Write-Host "PASS: $case"
        }
        finally { Pop-Location }
    }
}
finally {
    Pop-Location
    $env:GIT_CONFIG_GLOBAL = $previousGlobalConfig
    $env:GIT_CONFIG_NOSYSTEM = $previousSystemConfig
    Remove-Item -LiteralPath $root -Recurse -Force
}
exit 0
