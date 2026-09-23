$ErrorActionPreference = 'Stop'
$setup = Join-Path $PSScriptRoot '..\setup-pr-validation.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ("fsharp-pr-validation-" + [Guid]::NewGuid())
$git = (Get-Command git -CommandType Application).Source
$previousGlobalConfig = $env:GIT_CONFIG_GLOBAL
$previousSystemConfig = $env:GIT_CONFIG_NOSYSTEM

function Invoke-FixtureGit {
    $output = & $git @args
    if ($LASTEXITCODE -ne 0) { throw "Fixture git command failed: $args" }
    $output
}

function Invoke-RestMethod {
    param($Uri, $Headers)
    if ($Uri -ne 'https://api.github.com/repos/dotnet/fsharp/pulls/1') { throw "Unexpected URL: $Uri" }
    if ($case -eq 'api-failure') { throw 'GitHub unavailable' }
    $pr
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
    $remote = (Get-Location).Path.Replace('\', '/')
    Set-Location $root

    $cases = @{
        valid = $null
        'head-ref-changed' = 'current head/base refs'
        'base-ref-changed' = 'current head/base refs'
        'merge-mismatch' = 'fetched PR merge'
        'missing-merge' = 'git fetch'
        closed = 'PR is closed'
        'non-main' = 'PR is closed'
        'api-head-changed' = 'PR is closed'
        'api-base-changed' = 'PR is closed'
        'api-failure' = 'GitHub unavailable'
        'wrong-trusted-checkout' = 'trusted main checkout'
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
            $pr = @{
                state = 'open'
                head = @{ sha = $head }
                base = @{ sha = $base; ref = 'main'; repo = @{ full_name = 'dotnet/fsharp' } }
            }
            $expectedHead = $head
            switch ($case) {
                'head-ref-changed' { Invoke-FixtureGit -C $remote update-ref refs/pull/1/head $later }
                'base-ref-changed' { Invoke-FixtureGit -C $remote update-ref refs/heads/main $later }
                'merge-mismatch' { Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $later }
                'missing-merge' { Invoke-FixtureGit -C $remote update-ref -d refs/pull/1/merge }
                'closed' { $pr.state = 'closed' }
                'non-main' { $pr.base.ref = 'release/test' }
                'api-head-changed' { $pr.head.sha = $later }
                'api-base-changed' { $pr.base.sha = $later }
                'wrong-trusted-checkout' {
                    Invoke-FixtureGit fetch -q --depth=1 $remote refs/pull/1/head
                    Invoke-FixtureGit checkout -q --detach $head
                }
                'invalid-input' { $expectedHead = 'not-a-sha' }
            }
            $before = Invoke-FixtureGit rev-parse HEAD
            $failure = $null
            try { & $setup -prNumber 1 -headSha $expectedHead -baseSha $base }
            catch { $failure = "$_" }
            if ($null -eq $cases[$case]) {
                if ($failure) { throw "$case failed: $failure" }
                if ((Invoke-FixtureGit rev-parse HEAD) -ne $merge) { throw 'Did not check out the verified merge' }
                if ((& $git config --get-regexp 'http\..*extraheader')) { throw 'Persisted checkout credentials' }
            }
            else {
                if (!$failure -or $failure -notmatch $cases[$case]) { throw "$case failed for wrong reason: $failure" }
                if ((Invoke-FixtureGit rev-parse HEAD) -ne $before) { throw "$case changed HEAD before validation" }
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
