$ErrorActionPreference = 'Stop'
$apex = Join-Path $PSScriptRoot '..\pipelines\apex-integration'
$root = Join-Path $PSScriptRoot ("apex-git-fixture-" + [Guid]::NewGuid())
$installation = Join-Path $root 'Git'
$git = (Get-Command git.exe -CommandType Application | Select-Object -First 1).Source
$gitRoot = Split-Path (Split-Path $git -Parent) -Parent
$previousPath = $env:PATH
$previousWorkFolder = $env:AGENT_WORKFOLDER
$previousLoggingDirectory = $env:LoggingDirectory
$systemPath = "$env:SystemRoot\System32"
$powershell = "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe"
$yaml = Get-Content (Join-Path $apex 'stage.yml') -Raw
$bootstrap = [regex]::Match($yaml, '(?ms)displayName: Verify and check out PR snapshot\r?\n      scriptBlock: \|\r?\n(?<script>.*?)(?=      environmentVariables:)').Groups['script'].Value
if (!$bootstrap) { throw 'Could not find the source bootstrap operation.' }
$provision = ($bootstrap -split '        \$sourceRoot =', 2)[0].Replace('C:\Test\Git', $installation)
$case = 'valid'

function Import-Module {
    param($Name)
    if ($Name -ne 'DartLab.Modules.Git') { throw "Unexpected module: $Name" }
    if ($case -eq 'missing-module') { throw 'DartLab Git module unavailable' }
}

function Install-Git {
    param($InstallationDirectory, $Scope)
    if ($InstallationDirectory -ne $installation -or $Scope -ne 'Process') {
        throw 'Expected the bundled Git installer at the fixed destination with process scope.'
    }
    if ($case -eq 'installer-failure') { throw 'Bundled Git installation failed' }
    if (!(Test-Path $InstallationDirectory)) {
        New-Item -ItemType Junction -Path $InstallationDirectory -Target $gitRoot | Out-Null
    }
    $env:PATH = "$env:PATH;$(Join-Path $InstallationDirectory 'cmd')"
}

New-Item -ItemType Directory -Path $root | Out-Null
try {
    $env:PATH = $systemPath
    & ([scriptblock]::Create($provision))
    $expectedGit = Join-Path $installation 'cmd\git.exe'
    if ((Get-Command git.exe -CommandType Application | Select-Object -First 1).Source -ne $expectedGit) {
        throw 'Bootstrap did not select the provisioned Git.'
    }
    Write-Host 'PASS: bootstrap provisions bundled Git without initial Git PATH'
    $env:PATH = $previousPath
    & ([scriptblock]::Create($provision))
    if ((Get-Command git.exe -CommandType Application | Select-Object -First 1).Source -ne $expectedGit) {
        throw 'Bootstrap preferred ambient Git over the bundled distribution.'
    }
    Write-Host 'PASS: bootstrap prefers bundled Git over ambient Git'
    New-Item -Path Function: -Name $expectedGit -Value { $global:LASTEXITCODE = 23 } | Out-Null
    try {
        $failure = $null
        try { & ([scriptblock]::Create($provision)) }
        catch { $failure = "$_" }
        if (!$failure -or $failure -notmatch 'provisioned DartLab Git is not usable') {
            throw 'Bootstrap ignored a failing Git executable.'
        }
        Write-Host 'PASS: unusable provisioned Git stops bootstrap'
    }
    finally { Remove-Item -LiteralPath "Function:\$expectedGit" }

    $fixtureApex = Join-Path $root 'eng\pipelines\apex-integration'
    New-Item -ItemType Directory -Path $fixtureApex -Force | Out-Null
    (Get-Content (Join-Path $apex 'run-apex.ps1') -Raw).Replace('C:\Test\Git\cmd', (Join-Path $installation 'cmd')) |
        Set-Content (Join-Path $fixtureApex 'run-apex.ps1')
    Set-Content (Join-Path $root 'eng\SetupVSHive.ps1') @'
(Get-Command git.exe -CommandType Application).Source | Set-Content .\setup-git.txt
$env:AGENT_WORKFOLDER | Set-Content .\setup-workfolder.txt
'@
    Set-Content (Join-Path $root 'eng\CIBuildNoPublish.cmd') @'
@echo off
git --version >nul
if errorlevel 1 exit /b %ERRORLEVEL%
"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -Command "(Get-Command git.exe -CommandType Application).Source | Set-Content .\child-git.txt; $env:AGENT_WORKFOLDER | Set-Content .\child-workfolder.txt; New-Item -ItemType Directory artifacts\TestResults\Release -Force | Out-Null; Set-Content artifacts\TestResults\Release\fixture.trx '<TestRun />'"
exit /b %ERRORLEVEL%
'@
    & $git -C $root init -q
    & $git -C $root -c user.name=Test -c user.email=test@example.invalid commit -q --allow-empty -m fixture
    if ($LASTEXITCODE -ne 0) { throw 'Could not initialize the offline Apex repository.' }
    $env:LoggingDirectory = Join-Path $root 'cloudtest-results'
    foreach ($workFolder in @($null, (Join-Path $root 'stale-workspace'))) {
        $env:AGENT_WORKFOLDER = $workFolder
        $env:PATH = $systemPath
        & $powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $fixtureApex 'run-apex.ps1')
        if ($LASTEXITCODE -ne 0) { throw 'Independent Apex process failed.' }
        foreach ($file in @('setup-git.txt', 'child-git.txt')) {
            if ((Get-Content (Join-Path $root $file)) -ne $expectedGit) { throw "$file used the wrong Git." }
        }
        foreach ($file in @('setup-workfolder.txt', 'child-workfolder.txt')) {
            if ((Get-Content (Join-Path $root $file)) -ne $root) { throw "$file did not receive the repository workspace for SBOM packaging." }
        }
        if ($env:PATH -ne $systemPath -or $env:AGENT_WORKFOLDER -ne $workFolder) {
            throw 'Apex changed its parent process environment.'
        }
        Write-Host "PASS: independent Apex children use provisioned Git and repository workspace (caller workspace='$workFolder')"
    }

    [IO.Directory]::Delete($installation)
    foreach ($case in @('missing-module', 'installer-failure')) {
        $failure = $null
        try { & ([scriptblock]::Create($provision)) }
        catch { $failure = "$_" }
        $expected = if ($case -eq 'missing-module') { 'module unavailable' } else { 'installation failed' }
        if (!$failure -or $failure -notmatch $expected -or (Test-Path $installation)) {
            throw "$case did not fail closed: $failure"
        }
        Write-Host "PASS: $case stops bootstrap"
    }
    $env:PATH = $previousPath
    $run = Join-Path $fixtureApex 'run-apex.ps1'
    $output = & $powershell -NoProfile -ExecutionPolicy Bypass -Command "try { & '$run' } catch { Write-Output `$_; exit 1 }"
    if ($LASTEXITCODE -eq 0 -or "$output" -notmatch 'Git was not provisioned') {
        throw 'Apex fell back to ambient Git when provisioning was missing.'
    }
    Write-Host 'PASS: Apex rejects missing provisioned Git even with ambient Git available'
}
finally {
    $env:PATH = $previousPath
    $env:AGENT_WORKFOLDER = $previousWorkFolder
    $env:LoggingDirectory = $previousLoggingDirectory
    if (Test-Path $installation) { [IO.Directory]::Delete($installation) }
    Remove-Item -LiteralPath $root -Recurse -Force
}
