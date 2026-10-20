# Requires PowerShell 7 on Windows; compatible with Pester 3.x+.
# Run with: Invoke-Pester .\tests\ILVerify\ilverify.Artifacts.Tests.ps1

$pipeline = (Get-Content "$PSScriptRoot\..\..\azure-pipelines-PR.yml" -Raw).Replace("`r`n", "`n")
$jobs = [regex]::Matches($pipeline, '(?ms)^        - job: ILVerify\r?\n.*?(?=^ {0,8}\S|\z)')
if ($jobs.Count -ne 1) { throw "Expected exactly one ILVerify job." }
$steps = [regex]::Matches($jobs[0].Value, '(?ms)^ {10}- .*?(?=^ {10}- |\z)') | ForEach-Object { $_.Value.TrimEnd() }

function Get-ILVerifyStep([string]$Name) {
    $found = @($steps | Where-Object { $_ -match "(?m)^ {12}displayName: $([regex]::Escape($Name))\r?$" })
    if ($found.Count -ne 1) { throw "Expected one '$Name' step in job ILVerify; found $($found.Count)." }
    return $found[0]
}

function Invoke-Producer([string]$Scenario = 'mismatch', [string]$Root = "$caseRoot\repo") {
    $source = New-Item -ItemType Directory -Path "$Root\tests\ILVerify" -Force
    New-Item -ItemType Directory -Path "$Root\eng\common", "$Root\runtime\10.0.0" -Force | Out-Null
    Copy-Item -LiteralPath "$PSScriptRoot\ilverify.ps1" -Destination $source.FullName
    $buildExit = if ($Scenario -eq 'early failure') { 7 } else { 0 }
    Set-Content "$Root\build.cmd" "@exit /b $buildExit"
    Set-Content "$Root\eng\common\dotnet.ps1" @'
if ($args.Count -ne 3 -or $args[0] -ne 'msbuild' -or
    $args[2] -notin '--getProperty:FSharpNetCoreProductTargetFramework', '--getProperty:FSharpCoreShippedNetTargetFramework') {
    throw "Unexpected MSBuild request: $args"
}
'net10.0'
'@
    $selected = @('FSharp.Core_Debug_netstandard2.1')
    if ($Scenario -eq 'multiple') { $selected += 'FSharp.Compiler.Service_Release_netcoreapp', 'FSharp.Core_Release_netstandard2.0' }
    if ($Scenario -eq 'retry') { $selected = @('FSharp.Compiler.Service_Debug_netstandard2.0') }
    if ($Scenario -eq 'matching' -or $Scenario -eq 'early failure') { $selected = @() }
    $outputs = @{}
    $expected = @{}
    foreach ($project in 'FSharp.Core', 'FSharp.Compiler.Service') {
        $tfms = if ($project -eq 'FSharp.Core') { 'netstandard2.0', 'netstandard2.1', 'net10.0' } else { 'netstandard2.0', 'net10.0' }
        foreach ($configuration in 'Debug', 'Release') {
            foreach ($tfm in $tfms) {
                $baselineTfm = if ($tfm -eq 'net10.0') { 'netcoreapp' } else { $tfm }
                $id = "${project}_${configuration}_${baselineTfm}"
                $dllDirectory = New-Item -ItemType Directory -Path "$Root\artifacts\bin\$project\$configuration\$tfm" -Force
                $dll = New-Item -ItemType File -Path "$($dllDirectory.FullName)\$project.dll"
                $baseline = "$source\ilverify_$id.bsl"
                $outputs[$dll.FullName] = "Verified $id"
                Set-Content $baseline $outputs[$dll.FullName]
                if ($id -in $selected) {
                    $outputs[$dll.FullName] = "[IL]: Error [StackUnexpected]: [fixture.dll : Fixture::$id()][offset 0x00000001] Unexpected stack type."
                    $expected["ilverify_$id.bsl.actual"] = "[IL]: Error [StackUnexpected]: : Fixture::$id()][offset 0x00000001] Unexpected stack type."
                    switch ($Scenario) {
                        'missing baseline' { Remove-Item -LiteralPath $baseline }
                        'empty baseline' { [IO.File]::WriteAllBytes($baseline, [byte[]]@()) }
                        default { Set-Content $baseline '[IL]: Error [StackByRef]: : Fixture::M()] Expected ByRef.' }
                    }
                }
            }
        }
    }
    $outputs | ConvertTo-Json | Set-Content "$Root\outputs.json"
    Set-Content "$Root\run.ps1" @'
$ErrorActionPreference = 'Stop'
$env:TEST_UPDATE_BSL = $null
Set-Location $PSScriptRoot
$outputs = Get-Content .\outputs.json -Raw | ConvertFrom-Json -AsHashtable
function dotnet {
    $global:LASTEXITCODE = 0
    switch ($args -join ' ') {
        'ilverify --version' { return 'ILVerify fixture' }
        'tool list -g' { return 'dotnet-ilverify 10.0.0 ilverify' }
        '--list-runtimes' { return "Microsoft.NETCore.App 10.0.0 [$PSScriptRoot\runtime]" }
    }
    if ($args.Count -eq 8 -and ($args[0..2] -join ' ') -eq 'ilverify --sanity-checks --tokens' -and
        $args[4] -eq '-r' -and $args[6] -eq '-r') {
        $dll = [IO.Path]::GetFullPath($args[3])
        if ($outputs.ContainsKey($dll)) {
            if ($outputs[$dll] -match '^\[IL\]') { $global:LASTEXITCODE = 1 }
            return $outputs[$dll]
        }
    }
    throw "Unexpected dotnet request: $args"
}
& .\tests\ILVerify\ilverify.ps1
exit $LASTEXITCODE
'@
    $output = & "$PSHOME\pwsh.exe" -NoProfile -NonInteractive -File "$Root\run.ps1" 2>&1
    $producerExit = $LASTEXITCODE
    $expectedExit = if ($Scenario -eq 'matching') { 0 } else { 1 }
    $producerExit | Should Be $expectedExit
    $message = switch ($Scenario) {
        'matching' { 'ILverify output matches baseline.' }
        'early failure' { 'Build failed for Debug configuration (last exit code: 7).' }
        'missing baseline' { 'Baseline file not found:' }
        'empty baseline' { 'Baseline file is empty:' }
        default { 'ILverify output does not match baseline, differences:' }
    }
    ($output -join "`n") | Should Match ([regex]::Escape($message))
    $actuals = @(Get-ChildItem -LiteralPath $source.FullName -Filter '*.bsl.actual' -File)
    ($actuals.Name | Sort-Object) -join '|' | Should Be (($expected.Keys | Sort-Object) -join '|')
    foreach ($actual in $actuals) {
        (Get-Content -LiteralPath $actual.FullName -Raw) | Should Be ($expected[$actual.Name] + [Environment]::NewLine)
    }
    return [pscustomobject]@{ Root = $Root; Source = $source.FullName; Actuals = $actuals; ExitCode = $producerExit }
}

function Invoke-Collector([string]$Root, [int]$Attempt = 1) {
    $step = Get-ILVerifyStep 'Stage ILVerify actual baselines'
    $body = [regex]::Match($step, '(?ms)^ {10}- pwsh: \|\r?\n(?<body>(?: {14}[^\r\n]*\r?\n|\r?\n)+)')
    $body.Success | Should Be $true
    $script = $body.Groups['body'].Value -replace '(?m)^ {14}', ''
    $script = $script.Replace('$(Build.SourcesDirectory)', $Root).
        Replace('$(Build.ArtifactStagingDirectory)', "$caseRoot\staging").
        Replace('$(System.JobAttempt)', "$Attempt")
    $errors = $null
    [System.Management.Automation.Language.Parser]::ParseInput($script, [ref]$null, [ref]$errors) | Out-Null
    $errors | Should BeNullOrEmpty
    Set-Content "$caseRoot\collect.ps1" $script
    $output = & "$PSHOME\pwsh.exe" -NoProfile -NonInteractive -File "$caseRoot\collect.ps1" 2>&1
    $collectorExit = $LASTEXITCODE
    $ready = @($output | ForEach-Object {
        if ("$_" -match '^##vso\[task.setvariable variable=ILVerifyActualBaselinesFound\](true|false)$') { $Matches[1] }
    })
    return [pscustomobject]@{
        ExitCode = $collectorExit
        Output = $output -join "`n"
        Ready = $ready -join ','
        Destination = "$caseRoot\staging\ILVerifyActualBaselines-$Attempt"
    }
}

Describe 'ILVerify actual baseline artifacts' {
    BeforeEach {
        $caseRoot = (New-Item -ItemType Directory -Path "$TestDrive\$([guid]::NewGuid())").FullName
    }

    AfterEach {
        Remove-Item -LiteralPath $caseRoot -Recurse -Force
    }

    It 'runs the unchanged producer and generates the exact substantive mismatch actual' {
        $producer = Invoke-Producer
        $producer.ExitCode | Should Be 1
        $producer.Actuals.Count | Should Be 1
        $producer.Actuals[0].Name | Should Be 'ilverify_FSharp.Core_Debug_netstandard2.1.bsl.actual'
    }

    It 'wires only non-blocking, failure-only diagnostics immediately after Run ILVerify' {
        $run = Get-ILVerifyStep 'Run ILVerify'
        $run.Trim() | Should Be ('- pwsh: .\tests\ILVerify\ilverify.ps1' + "`n" +
            '            displayName: Run ILVerify' + "`n" +
            '            workingDirectory: $(Build.SourcesDirectory)')
        $collector = Get-ILVerifyStep 'Stage ILVerify actual baselines'
        $publisher = Get-ILVerifyStep 'Publish ILVerify actual baselines'
        [array]::IndexOf($steps, $collector) | Should Be ([array]::IndexOf($steps, $run) + 1)
        [array]::IndexOf($steps, $publisher) | Should Be ([array]::IndexOf($steps, $collector) + 1)
        foreach ($step in $collector, $publisher) {
            $step | Should Match '(?m)^ {12}continueOnError: true\r?$'
            $step | Should Not Match '(?m)^ {12}enabled: false\r?$'
        }
        $publisher | Should Match '(?m)^ {10}- task: PublishPipelineArtifact@1\r?$'
        $publisher | Should Match '(?m)^ {12}inputs:\r?$'
        $target = '$(Build.ArtifactStagingDirectory)\ILVerifyActualBaselines-$(System.JobAttempt)'
        $publisher | Should Match ('(?m)^ {14}targetPath: ' + [regex]::Escape("'$target'") + '\r?$')
        $publisher | Should Match ('(?m)^ {14}artifactName: ' + [regex]::Escape("'ILVerify actual baselines Attempt " + '$(System.JobAttempt)' + "'") + '\r?$')
        $collector | Should Match ([regex]::Escape($target))
        $collector | Should Match ([regex]::Escape("`$ErrorActionPreference = 'Stop'"))
    }

    # Azure evaluates these exact expressions; local tests do not emulate its scheduler.
    # https://learn.microsoft.com/azure/devops/pipelines/process/conditions
    # failed() still sees the original failed step after a successful diagnostic step.
    It 'requires failed job status even with stale true readiness; false or unset cannot publish' {
        $collector = Get-ILVerifyStep 'Stage ILVerify actual baselines'
        $publisher = Get-ILVerifyStep 'Publish ILVerify actual baselines'
        [regex]::Match($collector, '(?m)^ {12}condition: (.+?)\r?$').Groups[1].Value | Should Be 'failed()'
        [regex]::Match($publisher, '(?m)^ {12}condition: (.+?)\r?$').Groups[1].Value |
            Should Be "and(failed(), eq(variables['ILVerifyActualBaselinesFound'], 'true'))"
    }

    It 'collects exact producer files for <Scenario>' -TestCases @(
        @{ Scenario = 'mismatch' }
        @{ Scenario = 'missing baseline' }
        @{ Scenario = 'empty baseline' }
        @{ Scenario = 'multiple' }
        @{ Scenario = 'matching' }
        @{ Scenario = 'early failure' }
        @{ Scenario = 'boundary' }
        @{ Scenario = 'copy failure' }
    ) {
        param($Scenario)
        $producer = Invoke-Producer $Scenario
        if ($Scenario -eq 'boundary') {
            foreach ($name in 'unrelated.bsl', 'unrelated.dll', 'unrelated.binlog', 'notes.txt', 'unrelated.actual') {
                Set-Content "$($producer.Source)\$name" 'exclude me'
            }
            foreach ($name in 'something.bsl.actual', '.nuget\packages', 'nested') {
                $directory = New-Item -ItemType Directory -Path "$($producer.Source)\$name" -Force
                Set-Content "$directory\excluded.bsl.actual" 'exclude me'
            }
            Set-Content "$($producer.Root)\outside.bsl.actual" 'exclude me'
        }
        if ($Scenario -eq 'copy failure') {
            New-Item -ItemType Directory -Path "$caseRoot\staging" | Out-Null
            Set-Content "$caseRoot\staging\ILVerifyActualBaselines-1" 'not a directory'
        }
        $collected = Invoke-Collector $producer.Root
        if ($Scenario -eq 'copy failure') {
            $collected.ExitCode | Should Not Be 0
            $collected.Ready | Should Be 'false'
            $collected.Output | Should Match 'already exists'
            $producer.ExitCode | Should Be 1
        } else {
            $collected.ExitCode | Should Be 0
            if ($producer.Actuals.Count -eq 0) {
                $collected.Ready | Should Be 'false'
                $collected.Output | Should Match 'No ILVerify actual baselines'
                Test-Path -LiteralPath $collected.Destination | Should Be $false
            } else {
                $collected.Ready | Should Be 'false,true'
                $staged = @(Get-ChildItem -LiteralPath $collected.Destination -Recurse)
                ($staged.FullName | ForEach-Object { [IO.Path]::GetRelativePath($collected.Destination, $_) } | Sort-Object) -join '|' |
                    Should Be (($producer.Actuals.Name | Sort-Object) -join '|')
                foreach ($actual in $producer.Actuals) {
                    [Convert]::ToBase64String([IO.File]::ReadAllBytes("$($collected.Destination)\$($actual.Name)")) |
                        Should Be ([Convert]::ToBase64String([IO.File]::ReadAllBytes($actual.FullName)))
                }
                $producer.ExitCode | Should Be 1
            }
        }
    }

    It 'handles failure before the source directory exists without marking ready' {
        $collected = Invoke-Collector "$caseRoot\absent"
        $collected.ExitCode | Should Be 0
        $collected.Ready | Should Be 'false'
        $collected.Output | Should Match 'No ILVerify actual baselines'
        Test-Path -LiteralPath $collected.Destination | Should Be $false
    }

    It 'isolates files left by a previous job attempt' {
        $first = Invoke-Producer 'multiple' "$caseRoot\first"
        $previous = Invoke-Collector $first.Root 1
        $previous.ExitCode | Should Be 0
        $second = Invoke-Producer 'retry' "$caseRoot\second"
        $current = Invoke-Collector $second.Root 2
        $current.ExitCode | Should Be 0
        $current.Ready | Should Be 'false,true'
        @(Get-ChildItem -LiteralPath $previous.Destination -File).Count | Should Be 3
        @(Get-ChildItem -LiteralPath $current.Destination -File).Count | Should Be 1
        (Get-ChildItem -LiteralPath $current.Destination -File).Name | Should Be $second.Actuals[0].Name
    }
}
