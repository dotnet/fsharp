$ErrorActionPreference = 'Stop'
$root = Join-Path ([IO.Path]::GetTempPath()) ('fsharp-apex-results-' + [Guid]::NewGuid())
$apex = Join-Path $root 'eng\pipelines\apex-integration'
$git = (Get-Command git.exe -CommandType Application | Select-Object -First 1).Source
$powershell = "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe"
$previousLoggingDirectory = $env:LoggingDirectory
$previousCase = $env:APEX_RESULTS_CASE
New-Item -ItemType Directory -Path $apex -Force | Out-Null
try {
    (Get-Content "$PSScriptRoot\..\pipelines\apex-integration\run-apex.ps1" -Raw).Replace('C:\Test\Git\cmd', (Split-Path $git)) |
        Set-Content (Join-Path $apex 'run-apex.ps1')
    Set-Content (Join-Path $root 'eng\SetupVSHive.ps1') "if (`$env:APEX_RESULTS_CASE -eq 'setup-failure') { throw 'Fixture setup failed' }"
    Set-Content (Join-Path $root 'eng\CIBuildNoPublish.cmd') @'
@echo off
"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -File "%~dp0fixture-build.ps1" %*
exit /b %ERRORLEVEL%
'@
    Set-Content (Join-Path $root 'eng\fixture-build.ps1') @'
param([string]$Configuration, [switch]$deployExtensions, [switch]$testApex)
$ErrorActionPreference = 'Stop'
Set-Content build-ran.txt 'yes'
if ($env:APEX_RESULTS_CASE -eq 'build-before-results') { exit 7 }
if ($env:APEX_RESULTS_CASE -eq 'missing-results') { exit 0 }
$results = "artifacts\TestResults\$Configuration"
New-Item -ItemType Directory "$results\attachments" -Force | Out-Null
Set-Content "$results\diagnostic.txt" 'diagnostic'
Set-Content "$results\attachments\detail.txt" 'attachment'
if ($env:APEX_RESULTS_CASE -ne 'missing-trx') { Set-Content "$results\fixture.trx" '<TestRun />' }
if ($env:APEX_RESULTS_CASE -in 'build-after-results', 'build-and-copy-failure') { exit 9 }
exit 0
'@
    Set-Content (Join-Path $root 'invoke.ps1') @'
param([string]$Configuration)
try { & "$PSScriptRoot\eng\pipelines\apex-integration\run-apex.ps1" -Configuration $Configuration }
catch { Write-Output "PRIMARY: $($_.Exception.Message)"; exit 1 }
'@
    & $git -C $root init -q
    & $git -C $root -c user.name=Test -c user.email=test@example.invalid commit -q --allow-empty -m fixture
    if ($LASTEXITCODE -ne 0) { throw 'Could not initialize fixture repository.' }
    $cases = [ordered]@{
        success = ''
        'build-after-results' = 'Apex build and tests failed with exit code 9.'
        'build-before-results' = 'Apex build and tests failed with exit code 7.'
        'setup-failure' = 'Fixture setup failed'
        'missing-results' = 'Failed to hand off Apex results'
        'missing-trx' = 'Failed to hand off Apex results'
        'copy-failure' = 'Failed to hand off Apex results'
        'build-and-copy-failure' = 'Apex build and tests failed with exit code 9.'
        'missing-destination' = 'CloudTest LoggingDirectory is required'
        debug = ''
    }
    foreach ($case in $cases.Keys) {
        $configuration = if ($case -eq 'debug') { 'Debug' } else { 'Release' }
        $source = Join-Path $root "artifacts\TestResults\$configuration"
        $destination = Join-Path $root 'cloudtest-results'
        foreach ($path in @($source, $destination, (Join-Path $root 'build-ran.txt'))) {
            if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path -Recurse -Force }
        }
        if ($case -in 'copy-failure', 'build-and-copy-failure') { Set-Content $destination 'blocked' }
        $env:LoggingDirectory = if ($case -eq 'missing-destination') { $null } else { $destination }
        $env:APEX_RESULTS_CASE = $case
        $ErrorActionPreference = 'Continue'
        $output = @(& $powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $root 'invoke.ps1') -Configuration $configuration 2>&1)
        $code = $LASTEXITCODE
        $ErrorActionPreference = 'Stop'
        $text = $output -join "`n"
        if ($cases[$case]) {
            if ($code -eq 0 -or !$text.Contains("PRIMARY: $($cases[$case])")) { throw "$case did not preserve its expected error: $text" }
        }
        elseif ($code -ne 0) { throw "$case failed: $text" }
        $expectTrx = $case -in 'success', 'build-after-results', 'debug'
        if ((Test-Path (Join-Path $destination 'fixture.trx')) -ne $expectTrx) { throw "$case has an incorrect TRX handoff" }
        if ($expectTrx) {
            foreach ($file in 'fixture.trx', 'diagnostic.txt', 'attachments\detail.txt') {
                if ((Get-Content (Join-Path $destination $file) -Raw) -cne (Get-Content (Join-Path $source $file) -Raw)) { throw "$case changed $file" }
            }
        }
        if (($case -in 'missing-results', 'missing-trx') -and
            (!$text.Contains('no TRX results were produced') -or !$text.Contains($source))) { throw 'Missing expected result-path diagnostic' }
        if ($case -eq 'missing-trx' -and !(Test-Path (Join-Path $destination 'diagnostic.txt'))) { throw 'Diagnostics were not copied without TRX' }
        if ($case -eq 'build-and-copy-failure' -and !$text.Contains('Failed to hand off Apex results')) { throw 'Copy failure was not reported separately' }
        if (($case -in 'setup-failure', 'missing-destination') -and (Test-Path (Join-Path $root 'build-ran.txt'))) { throw "$case unexpectedly ran the build" }
        Write-Host "PASS: $case"
    }
}
finally {
    $env:LoggingDirectory = $previousLoggingDirectory
    $env:APEX_RESULTS_CASE = $previousCase
    Remove-Item -LiteralPath $root -Recurse -Force
}
