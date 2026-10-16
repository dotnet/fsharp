$versionProject = Join-Path $PSScriptRoot '..\VSVersion.proj'

function Invoke-VersionEvaluation {
    param ([hashtable]$Properties = @{}, [string]$Project = $versionProject, [switch]$EvaluationOnly)

    $arguments = @('msbuild', $Project, '-nologo', '-warnaserror', '-p:VSBuildTimestampUtc=')
    $arguments += if ($EvaluationOnly) { '-getProperty:VSMinorVersion,VSMinorVersionIsHeuristic' } else { '-t:PrintVSVersion' }
    foreach ($name in $Properties.Keys) {
        $arguments += "-p:$name=$($Properties[$name])"
    }
    $output = & dotnet @arguments 2>&1
    [PSCustomObject]@{ ExitCode = $LASTEXITCODE; Output = $output -join "`n" }
}

Describe 'Heuristic VS minor versions' {
    $cases = @()
    $transitions = @(
        @{ At = '2026-05-02T19:00:00Z'; Minor = 8 },
        @{ At = '2026-05-30T19:00:00Z'; Minor = 9 },
        @{ At = '2026-07-04T19:00:00Z'; Minor = 10 },
        @{ At = '2026-08-01T19:00:00Z'; Minor = 11 },
        @{ At = '2026-08-29T19:00:00Z'; Minor = 12 },
        @{ At = '2026-10-03T19:00:00Z'; Minor = 13 },
        @{ At = '2026-10-31T19:00:00Z'; Minor = 14 },
        @{ At = '2026-11-28T20:00:00Z'; Minor = 15 },
        @{ At = '2027-01-02T20:00:00Z'; Minor = 16 },
        @{ At = '2027-02-27T20:00:00Z'; Minor = 18 },
        @{ At = '2027-04-03T19:00:00Z'; Minor = 19 },
        @{ At = '2028-03-04T20:00:00Z'; Minor = 30 }
    )
    foreach ($transition in $transitions) {
        $at = [DateTimeOffset]::Parse($transition.At)
        foreach ($ticks in @(-1, 0, 1)) {
            $cases += @{
                Timestamp = $at.AddTicks($ticks).ToString('o')
                Minor = $transition.Minor - [int]($ticks -lt 0)
            }
        }
    }

    It 'selects minor <Minor> at <Timestamp>' -TestCases $cases {
        param ($Timestamp, $Minor)
        $result = Invoke-VersionEvaluation @{ VSBuildTimestampUtc = $Timestamp }
        $result.ExitCode | Should Be 0
        $result.Output | Should Match "VSMinorVersion=$Minor; VSAssemblyVersion=18\.$Minor\.0\.0"
        $result.Output | Should Match 'warning FSVS1001'
    }

    It 'uses the literal local minor without a timestamp' {
        $result = Invoke-VersionEvaluation
        $result.ExitCode | Should Be 0
        $result.Output | Should Match 'VSMinorVersion=8;'
        $result.Output | Should Not Match 'FSVS1001'
    }

    It 'retains the minor during the 24-hour delay at <Timestamp>' -TestCases @(
        @{ Timestamp = '2026-08-28T19:00:00Z'; Minor = 11 },
        @{ Timestamp = '2026-07-03T19:00:00Z'; Minor = 9 },
        @{ Timestamp = '2026-10-30T19:00:00Z'; Minor = 13 },
        @{ Timestamp = '2026-11-27T20:00:00Z'; Minor = 14 }
    ) {
        param ($Timestamp, $Minor)
        $result = Invoke-VersionEvaluation @{ VSBuildTimestampUtc = $Timestamp }
        $result.ExitCode | Should Be 0
        $result.Output | Should Match "VSMinorVersion=$Minor;"
    }

    It 'normalizes equivalent offsets including winter offsets' -TestCases @(
        @{ Timestamp = '2026-08-29T12:00:00-07:00'; Minor = 12 },
        @{ Timestamp = '2026-08-29T21:00:00+02:00'; Minor = 12 },
        @{ Timestamp = '2026-11-28T12:00:00-08:00'; Minor = 15 },
        @{ Timestamp = '2026-10-31T12:00:00-07:00'; Minor = 14 }
    ) {
        param ($Timestamp, $Minor)
        $result = Invoke-VersionEvaluation @{ VSBuildTimestampUtc = $Timestamp }
        $result.ExitCode | Should Be 0
        $result.Output | Should Match "VSMinorVersion=$Minor;"
    }

    It 'keeps a fixed backport minor with a later timestamp' {
        $result = Invoke-VersionEvaluation @{
            UseVSScheduledMinorVersion = 'false'
            VSMinorVersion = '11'
            VSBuildTimestampUtc = '2026-12-02T19:00:00Z'
        }
        $result.ExitCode | Should Be 0
        $result.Output | Should Match 'VSMinorVersion=11;'
        $result.Output | Should Not Match 'FSVS1001'
    }

    It 'rejects invalid input <Timestamp>' -TestCases @(
        @{ Timestamp = 'garbage' },
        @{ Timestamp = '2026-09-11T12:00:00' },
        @{ Timestamp = '2024-01-01T00:00:00Z' },
        @{ Timestamp = '9999-12-31T19:00:00Z' }
    ) {
        param ($Timestamp)
        (Invoke-VersionEvaluation @{ VSBuildTimestampUtc = $Timestamp }).ExitCode | Should Not Be 0
    }

    It 'requires a deliberate calibration update for another VS major' {
        $result = Invoke-VersionEvaluation @{ VSMajorVersion = 19; VSBuildTimestampUtc = '2026-09-11T12:00:00Z' }
        $result.ExitCode | Should Not Be 0
        $result.Output | Should Match 'covers major 18 only'
    }

    It 'rejects computed minors outside the assembly component range' {
        $result = Invoke-VersionEvaluation @{ VSBuildTimestampUtc = '8000-01-01T00:00:00Z' }
        $result.ExitCode | Should Not Be 0
        $result.Output | Should Match 'exceeds 65534'
    }

    It 'rejects invalid input during evaluation alone with Arcade disabled=<Disabled>' -TestCases @(
        @{ Disabled = 'true' },
        @{ Disabled = 'false' }
    ) {
        param ($Disabled)
        $project = Join-Path $PSScriptRoot '..\..\..\vsintegration\src\FSharp.ProjectSystem.FSharp\FSharp.ProjectSystem.FSharp.fsproj'
        foreach ($invalid in @(
            @{ VSBuildTimestampUtc = 'garbage' },
            @{ VSBuildTimestampUtc = '2026-08-29T20:30:00' },
            @{ VSBuildTimestampUtc = '2024-01-01T00:00:00Z' },
            @{ VSBuildTimestampUtc = '8000-01-01T00:00:00Z' },
            @{ VSBuildTimestampUtc = '2026-09-11T12:00:00Z'; VSMajorVersion = '19' }
        )) {
            $invalid.DISABLE_ARCADE = $Disabled
            $result = Invoke-VersionEvaluation $invalid -Project $project -EvaluationOnly
            $result.ExitCode | Should Not Be 0
        }
    }

    It 'exposes heuristic status and normalizes offsets during evaluation alone' {
        foreach ($timestamp in @('2026-08-29T19:00:00Z', '2026-08-29T21:00:00+02:00', '2026-08-29T12:00:00-07:00')) {
            $result = Invoke-VersionEvaluation @{ VSBuildTimestampUtc = $timestamp } -EvaluationOnly
            $result.ExitCode | Should Be 0
            $properties = ($result.Output | ConvertFrom-Json).Properties
            $properties.VSMinorVersion | Should Be '12'
            $properties.VSMinorVersionIsHeuristic | Should Be 'true'
        }
        foreach ($fixed in @(
            @{},
            @{ UseVSScheduledMinorVersion = 'false'; VSMinorVersion = '11'; VSBuildTimestampUtc = 'not-a-timestamp' }
        )) {
            $result = Invoke-VersionEvaluation $fixed -EvaluationOnly
            $result.ExitCode | Should Be 0
            ($result.Output | ConvertFrom-Json).Properties.VSMinorVersionIsHeuristic | Should Be 'false'
        }
    }

    It 'propagates the selected minor to VS redirects with Arcade disabled=<Disabled>' -TestCases @(
        @{ Disabled = 'true' },
        @{ Disabled = 'false' }
    ) {
        param ($Disabled)
        $project = Join-Path $PSScriptRoot '..\..\..\vsintegration\src\FSharp.ProjectSystem.FSharp\FSharp.ProjectSystem.FSharp.fsproj'
        $output = & dotnet msbuild $project -nologo "-p:DISABLE_ARCADE=$Disabled" `
            -p:VSBuildTimestampUtc=2026-09-11T12:00:00Z `
            -getProperty:VSAssemblyVersion,FSCoreVersion,FSharpCompilerServiceVersion -getItem:AssemblyAttribute
        $LASTEXITCODE | Should Be 0
        $data = ($output -join "`n") | ConvertFrom-Json
        $data.Properties.VSAssemblyVersion | Should Be '18.12.0.0'
        $redirects = @($data.Items.AssemblyAttribute | Where-Object {
            $_.Identity -eq 'Microsoft.VisualStudio.Shell.ProvideBindingRedirectionAttribute' -and
            $_.AssemblyName -ne 'FSharp.Core'
        })
        $redirects.Count | Should Be 2
        foreach ($redirect in $redirects) {
            $redirect.OldVersionUpperBound | Should Be '18.12.0.0'
            $redirect.NewVersion | Should Be '18.12.0.0'
        }
        $baseline = & dotnet msbuild $project -nologo "-p:DISABLE_ARCADE=$Disabled" `
            -p:VSBuildTimestampUtc= -getProperty:FSCoreVersion,FSharpCompilerServiceVersion
        $LASTEXITCODE | Should Be 0
        $unchanged = ($baseline -join "`n") | ConvertFrom-Json
        $data.Properties.FSCoreVersion | Should Be $unchanged.Properties.FSCoreVersion
        $data.Properties.FSharpCompilerServiceVersion | Should Be $unchanged.Properties.FSharpCompilerServiceVersion
    }
}
