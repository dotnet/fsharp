$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path "$PSScriptRoot\..\..").Path
$ast = [System.Management.Automation.Language.Parser]::ParseFile("$repoRoot\eng\Build.ps1", [ref]$null, [ref]$null)
$runner = $ast.Find({ $args[0] -is [System.Management.Automation.Language.FunctionDefinitionAst] -and $args[0].Name -eq 'TestUsingModules' }, $false)
Invoke-Expression $runner.Extent.Text

function InitializeDotNetCli { Join-Path $repoRoot '.dotnet' }
function Exec-Console([string]$command, [string]$commandArgs) { throw 'Unexpected execution' }
function TestFromManifest([string]$platform, [string]$selection) { throw 'Unexpected execution' }

Describe 'Full_Signed test modules' {
    BeforeEach {
        $RepoRoot = $repoRoot
        $ArtifactsDir = Join-Path $TestDrive 'artifacts with spaces'
        $configuration = 'Release'
        $script:desktopTargetFramework = 'net472'
        $script:coreclrTargetFramework = 'net11.0'
        Mock Exec-Console {}
    }

    It 'runs each platform once with only its selected modules' {
        foreach ($platform in @('desktop', 'coreclr')) {
            $framework = if ($platform -eq 'desktop') { $desktopTargetFramework } else { $coreclrTargetFramework }
            $extension = if ($platform -eq 'desktop') { 'exe' } else { 'dll' }
            $commands = & "$repoRoot\.dotnet\dotnet.exe" fsi "$repoRoot\eng\tests\TestSplit.fsx" --all $platform
            if ($LASTEXITCODE -ne 0) { throw 'Manifest failed' }
            $expected = @($commands | ForEach-Object {
                $projectName = [IO.Path]::GetFileNameWithoutExtension(($_ -split ' ')[2])
                $module = "$projectName\$configuration\$framework\$projectName.$extension"
                $path = Join-Path "$ArtifactsDir\bin" $module
                New-Item -ItemType Directory -Path (Split-Path $path) -Force | Out-Null
                Set-Content -LiteralPath $path ''
                $module
            })
            # A stray server executable must never be picked up by desktop discovery.
            $server = "$ArtifactsDir\bin\FSharp.Compiler.Interactive.Server.Tests\$configuration\$framework"
            New-Item -ItemType Directory -Path $server -Force | Out-Null
            Set-Content "$server\FSharp.Compiler.Interactive.Server.Tests.$extension" ''

            TestUsingModules -platform $platform

            Assert-MockCalled Exec-Console -Times 1 -Exactly -Scope It -ParameterFilter {
                $commandArgs.Contains("--test-modules ""$($expected -join ';')""") -and
                $commandArgs.Contains("--root-directory ""$ArtifactsDir\bin""") -and
                $commandArgs.Contains("--results-directory ""$ArtifactsDir\TestResults\$configuration""") -and
                $commandArgs.Contains('--report-spekt-xunit-filename "{assembly}.{framework}.') -and
                $commandArgs.Contains('--hangdump --hangdump-timeout 5m --hangdump-type Full') -and
                $commandArgs -notmatch '--(project|solution|configuration|framework|no-build|no-restore)\b|/bl:' -and
                (($platform -eq 'coreclr') -eq $commandArgs.Contains('FSharp.Compiler.Interactive.Server.Tests'))
            }
        }
        Assert-MockCalled Exec-Console -Times 2 -Exactly -Scope It
    }

    It 'fails before running any tests if a selected module is missing' {
        $ArtifactsDir = Join-Path $TestDrive 'missing modules'
        { TestUsingModules -platform desktop } | Should Throw 'Test module not found:'
        Assert-MockCalled Exec-Console -Times 0 -Exactly -Scope It
    }

    It 'propagates runner failures' {
        Mock Test-Path { $true }
        Mock Exec-Console { throw 'Test process failed' }
        { TestUsingModules -platform coreclr } | Should Throw 'Test process failed'
        Assert-MockCalled Exec-Console -Times 1 -Exactly -Scope It
    }

    It 'preserves desktop runner selection' -TestCases @(
        @{ modules = $false; batch = ''; expectedModules = 0; expectedManifest = 1 }
        @{ modules = $true; batch = ''; expectedModules = 1; expectedManifest = 0 }
        @{ modules = $true; batch = '2'; expectedModules = 0; expectedManifest = 1 }
    ) {
        param($modules, $batch, $expectedModules, $expectedManifest)
        $testDesktop = $true
        $testUsingModules = $modules
        $testDesktopBatch = $batch
        Mock TestFromManifest {}
        Mock TestUsingModules {}
        $dispatch = $ast.Find({
            $args[0] -is [System.Management.Automation.Language.IfStatementAst] -and
            $args[0].Extent.Text.StartsWith('if ($testDesktop)')
        }, $true)

        & ([scriptblock]::Create($dispatch.Extent.Text))

        Assert-MockCalled TestUsingModules -Times $expectedModules -Exactly -Scope It
        Assert-MockCalled TestFromManifest -Times $expectedManifest -Exactly -Scope It -ParameterFilter {
            $platform -eq 'desktop' -and (!$batch -or $selection -eq $batch)
        }
    }
}
