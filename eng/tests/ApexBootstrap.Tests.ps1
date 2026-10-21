$ErrorActionPreference = 'Stop'
$root = Join-Path ([IO.Path]::GetTempPath()) ('fsharp-apex-bootstrap-' + [Guid]::NewGuid())
$git = (Get-Command git.exe -CommandType Application | Select-Object -First 1).Source
$gitRoot = Split-Path (Split-Path $git -Parent) -Parent
$installation = Join-Path $root 'Git'
$remote = Join-Path $root 'remote'
$source = Join-Path $root 'source'
$control = Join-Path $root 'control'
$previousEnvironment = @{}
foreach ($name in 'PATH', 'GIT_CONFIG_GLOBAL', 'GIT_CONFIG_NOSYSTEM', 'PIPELINE_SOURCE_SHA', 'PIPELINE_SOURCE_REF', 'HEAD_SHA', 'PR_NUMBER', 'LoggingDirectory') {
    $previousEnvironment[$name] = [Environment]::GetEnvironmentVariable($name)
}

function Invoke-FixtureGit {
    $output = & $git @args
    if ($LASTEXITCODE -ne 0) { throw "Fixture Git failed: $args" }
    $output
}

function Import-Module {
    param($Name)
    if ($Name -ne 'DartLab.Modules.Git') { throw "Unexpected module: $Name" }
}

function Install-Git {
    param($InstallationDirectory, $Scope)
    if ($InstallationDirectory -ne $installation -or $Scope -ne 'Process') { throw 'Unexpected Git installation' }
    if (!(Test-Path $installation)) { New-Item -ItemType Junction -Path $installation -Target $gitRoot | Out-Null }
}

function Invoke-RestMethod {
    param($Uri, $Headers)
    if ($Uri -eq 'https://api.github.com/repos/dotnet/fsharp/pulls/1/commits?per_page=100&page=1') {
        Write-Output -NoEnumerate @([pscustomobject]@{ sha = $reviewed })
        return
    }
    if ($Uri -ne 'https://api.github.com/repos/dotnet/fsharp/pulls/1') { throw "Unexpected URL: $Uri" }
    @{ state = 'open'; head = @{ sha = $currentHead }; base = @{ sha = $main; ref = 'main'; repo = @{ full_name = 'dotnet/fsharp' } } }
}

New-Item -ItemType Directory $root | Out-Null
Push-Location $root
try {
    $env:GIT_CONFIG_GLOBAL = Join-Path $root 'gitconfig'
    $env:GIT_CONFIG_NOSYSTEM = '1'
    Invoke-FixtureGit config --global "url.$($remote.Replace('\', '/')).insteadOf" 'https://github.com/dotnet/fsharp.git'
    Invoke-FixtureGit init -q -b main $remote
    Invoke-FixtureGit -C $remote config user.name Test
    Invoke-FixtureGit -C $remote config user.email test@example.invalid
    $scripts = Join-Path $remote 'eng\pipelines\apex-integration'
    New-Item -ItemType Directory $scripts -Force | Out-Null
    Copy-Item "$PSScriptRoot\..\setup-pr-validation.ps1" (Join-Path $remote 'eng')
    $runner = (Get-Content "$PSScriptRoot\..\pipelines\apex-integration\run-apex.ps1" -Raw).Replace('C:\Test\Git\cmd', (Join-Path $installation 'cmd'))
    Set-Content (Join-Path $scripts 'run-apex.ps1') $runner
    Set-Content (Join-Path $remote 'eng\SetupVSHive.ps1') "Set-Content .\product-setup.txt 'PR product setup'"
    Set-Content (Join-Path $remote 'eng\CIBuildNoPublish.cmd') @'
@echo off
mkdir artifacts\TestResults\Release
echo ^<TestRun /^> >artifacts\TestResults\Release\fixture.trx
exit /b 0
'@
    Invoke-FixtureGit -C $remote add .
    Invoke-FixtureGit -C $remote commit -q -m orchestration
    $main = Invoke-FixtureGit -C $remote rev-parse HEAD
    Invoke-FixtureGit -C $remote checkout -q -b feature
    Set-Content (Join-Path $remote 'eng\setup-pr-validation.ps1') "throw 'PR verifier must not execute'"
    Set-Content (Join-Path $scripts 'run-apex.ps1') "throw 'PR runner must not execute'"
    Invoke-FixtureGit -C $remote add .
    Invoke-FixtureGit -C $remote commit -q -m reviewed
    $reviewed = Invoke-FixtureGit -C $remote rev-parse HEAD
    Invoke-FixtureGit -C $remote commit -q --allow-empty -m 'newer PR push'
    $newHead = Invoke-FixtureGit -C $remote rev-parse HEAD
    Invoke-FixtureGit -C $remote update-ref refs/heads/feature $reviewed
    Invoke-FixtureGit -C $remote update-ref refs/pull/1/head $reviewed
    Invoke-FixtureGit -C $remote checkout -q main
    Invoke-FixtureGit -C $remote commit -q --allow-empty -m 'advance main after queueing'
    $advancedMain = Invoke-FixtureGit -C $remote rev-parse HEAD
    Invoke-FixtureGit -C $remote merge -q --no-ff feature -m integration
    $merge = Invoke-FixtureGit -C $remote rev-parse HEAD
    Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $merge
    Invoke-FixtureGit -C $remote update-ref refs/heads/main $advancedMain

    $yaml = Get-Content "$PSScriptRoot\..\pipelines\apex-integration\stage.yml" -Raw
    $bootstrap = [regex]::Match($yaml, '(?ms)displayName: Verify and check out PR snapshot\r?\n      scriptBlock: \|\r?\n(?<script>.*?)(?=      environmentVariables:)').Groups['script'].Value
    if (!$bootstrap -or !$yaml.Contains('-File C:\Test\FSharpControl\run-apex.ps1') -or !$yaml.Contains('-SourceRoot C:\Test\FSharp')) {
        throw 'Expected separate control runner and source workspace'
    }
    $bootstrap = $bootstrap.Replace('C:\Test\FSharpControl', $control).Replace('C:\Test\FSharp', $source).Replace('C:\Test\Git', $installation)
    $env:PR_NUMBER = '1'
    $env:HEAD_SHA = $reviewed
    $cases = [ordered]@{
        'main-advanced' = ''
        'older-reviewed-commit' = ''
        'existing-source' = 'unverified checkout'
        'existing-control' = 'unverified control scripts'
        'invalid-source-ref' = 'Expected pinned main'
        'missing-main-commit' = 'pinned main orchestration revision failed'
        'missing-reviewed-commit' = 'git fetch'
        'missing-control-script' = 'does not exist'
    }
    foreach ($case in $cases.Keys) {
        foreach ($path in @($source, $control)) {
            if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path -Recurse -Force }
        }
        $env:PIPELINE_SOURCE_SHA = $main
        $env:PIPELINE_SOURCE_REF = 'refs/heads/main'
        $env:HEAD_SHA = $reviewed
        $currentHead = $reviewed
        $expectedTested = $merge
        Invoke-FixtureGit -C $remote update-ref refs/pull/1/head $reviewed
        Invoke-FixtureGit -C $remote update-ref refs/pull/1/merge $merge
        if ($case -eq 'older-reviewed-commit') {
            $currentHead = $newHead
            $expectedTested = $reviewed
            Invoke-FixtureGit -C $remote update-ref refs/pull/1/head $newHead
            Invoke-FixtureGit -C $remote update-ref -d refs/pull/1/merge
        }
        if ($case -eq 'existing-source') { New-Item -ItemType Directory $source | Out-Null }
        if ($case -eq 'existing-control') { New-Item -ItemType Directory $control | Out-Null }
        if ($case -eq 'invalid-source-ref') { $env:PIPELINE_SOURCE_REF = 'refs/pull/1/head' }
        if ($case -eq 'missing-main-commit') { $env:PIPELINE_SOURCE_SHA = 'e' * 40 }
        if ($case -eq 'missing-reviewed-commit') { $env:HEAD_SHA = 'e' * 40 }
        if ($case -eq 'missing-control-script') {
            Invoke-FixtureGit -C $remote checkout -q --detach $main
            Invoke-FixtureGit -C $remote rm -q -f eng/setup-pr-validation.ps1
            Invoke-FixtureGit -C $remote commit -q -m 'missing verifier'
            $env:PIPELINE_SOURCE_SHA = Invoke-FixtureGit -C $remote rev-parse HEAD
        }
        $failure = $null
        try { & ([scriptblock]::Create($bootstrap)) }
        catch { $failure = "$_" }
        if ($cases[$case]) {
            if (!$failure -or $failure -notmatch $cases[$case]) { throw "$case failed for the wrong reason: $failure" }
        }
        else {
            if ($failure) { throw $failure }
            if ((Invoke-FixtureGit -C $source rev-parse HEAD) -ne $expectedTested) { throw 'Incorrect selected test commit' }
            if ((Get-Content (Join-Path $control 'run-apex.ps1') -Raw).TrimEnd() -cne $runner.TrimEnd()) {
                throw 'Runner did not come from pinned main'
            }
            $env:LoggingDirectory = Join-Path $root 'results'
            & "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy Bypass -File (Join-Path $control 'run-apex.ps1') -SourceRoot $source
            if ($LASTEXITCODE -ne 0 -or !(Test-Path (Join-Path $env:LoggingDirectory 'fixture.trx')) -or
                !(Test-Path (Join-Path $source 'product-setup.txt'))) { throw 'Trusted runner did not execute the selected product build' }
        }
        Write-Host "PASS: $case"
    }
}
finally {
    Pop-Location
    foreach ($name in $previousEnvironment.Keys) { [Environment]::SetEnvironmentVariable($name, $previousEnvironment[$name]) }
    if (Test-Path $installation) { [IO.Directory]::Delete($installation) }
    Remove-Item -LiteralPath $root -Recurse -Force
}
