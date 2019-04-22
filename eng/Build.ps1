#
# This script controls the F# build process. This encompasess everything from build, testing to
# publishing of NuGet packages. The intent is to structure it to allow for a simple flow of logic
# between the following phases:
#
#   - restore
#   - build
#   - sign
#   - pack
#   - test
#   - publish
#
# Each of these phases has a separate command which can be executed independently. For instance
# it's fine to call `build.ps1 -build -testDesktop` followed by repeated calls to
# `.\build.ps1 -testDesktop`.

[CmdletBinding(PositionalBinding=$false)]
param (
    [string][Alias('c')]$configuration = "Debug",
    [string][Alias('v')]$verbosity = "m",
    [string]$msbuildEngine = "vs",

    # Actions
    [switch][Alias('r')]$restore,
    [switch]$noRestore,
    [switch][Alias('b')]$build,
    [switch]$rebuild,
    [switch]$sign,
    [switch]$noSign,
    [switch]$pack,
    [switch]$publish,
    [switch]$launch,
    [switch]$help,

    # Options
    [switch][Alias('proto')]$bootstrap,
    [string]$bootstrapConfiguration = "Proto",
    [string]$bootstrapTfm = "net472",
    [switch][Alias('bl')]$binaryLog,
    [switch]$ci,
    [switch]$official,
    [switch]$procdump,
    [switch]$deployExtensions,
    [switch]$prepareMachine,
    [switch]$useGlobalNuGetCache = $true,
    [switch]$warnAsError = $true,
    [switch][Alias('test')]$testDesktop,
    [switch]$testCoreClr,
    [switch]$testFSharpCompiler,
    [switch]$testFSharpQA,
    [switch]$testFSharpCore,
    [switch]$testVs,
    [switch]$testAll,

    [parameter(ValueFromRemainingArguments=$true)][string[]]$properties)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

function Print-Usage() {
    Write-Host "Common settings:"
    Write-Host "  -configuration <value>    Build configuration: 'Debug' or 'Release' (short: -c)"
    Write-Host "  -verbosity <value>        Msbuild verbosity: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]"
    Write-Host "  -deployExtensions         Deploy built vsixes"
    Write-Host "  -binaryLog                Create MSBuild binary log (short: -bl)"
    Write-Host ""
    Write-Host "Actions:"
    Write-Host "  -restore                  Restore packages (short: -r)"
    Write-Host "  -build                    Build main solution (short: -b)"
    Write-Host "  -rebuild                  Rebuild main solution"
    Write-Host "  -pack                     Build NuGet packages, VS insertion manifests and installer"
    Write-Host "  -sign                     Sign our binaries"
    Write-Host "  -publish                  Publish build artifacts (e.g. symbols)"
    Write-Host "  -launch                   Launch Visual Studio in developer hive"
    Write-Host "  -help                     Print help and exit"
    Write-Host ""
    Write-Host "Test actions"
    Write-Host "  -testAll                  Run all tests"
    Write-Host "  -testDesktop              Run tests against full .NET Framework"
    Write-Host "  -testCoreClr              Run tests against CoreCLR"
    Write-Host "  -testFSharpCompiler       Run F# Compiler unit tests"
    Write-Host "  -testFSharpQA             Run F# Cambridge tests"
    Write-Host "  -testFSharpCore           Run FSharpCore unit tests"
    Write-Host "  -testVs                   Run F# editor unit tests"
    Write-Host ""
    Write-Host "Advanced settings:"
    Write-Host "  -ci                       Set when running on CI server"
    Write-Host "  -official                 Set when building an official build"
    Write-Host "  -bootstrap                Build using a bootstrap compiler"
    Write-Host "  -msbuildEngine <value>    Msbuild engine to use to run build ('dotnet', 'vs', or unspecified)."
    Write-Host "  -procdump                 Monitor test runs with procdump"
    Write-Host "  -prepareMachine           Prepare machine for CI run, clean up processes after build"
    Write-Host "  -useGlobalNuGetCache      Use global NuGet cache."
    Write-Host ""
    Write-Host "Command line arguments starting with '/p:' are passed through to MSBuild."
}

# Process the command line arguments and establish defaults for the values which are not
# specified.
function Process-Arguments() {
    if ($help -or (($properties -ne $null) -and ($properties.Contains("/help") -or $properties.Contains("/?")))) {
       Print-Usage
       exit 0
    }

    if ($testAll) {
        $script:testDesktop = $True
        $script:testCoreClr = $True
        $script:testFSharpQA = $True
        $script:testVs = $True
    }

    if ($noRestore) {
        $script:restore = $False;
    }

    if ($noSign) {
        $script:sign = $False;
    }

    foreach ($property in $properties) {
        if (!$property.StartsWith("/p:", "InvariantCultureIgnoreCase")) {
            Write-Host "Invalid argument: $property"
            Print-Usage
            exit 1
        }
    }
}

function Update-Arguments() {
    if (-Not (Test-Path "$ArtifactsDir\Bootstrap\fsc.exe")) {
        $script:bootstrap = $True
    }
}

function BuildSolution() {
    # VisualFSharp.sln can't be built with dotnet due to WPF, WinForms and VSIX build task dependencies
    $solution = "VisualFSharp.sln"

    Write-Host "$($solution):"

    $bl = if ($binaryLog) { "/bl:" + (Join-Path $LogDir "Build.binlog") } else { "" }
    $projects = Join-Path $RepoRoot $solution
    $officialBuildId = if ($official) { $env:BUILD_BUILDNUMBER } else { "" }
    $toolsetBuildProj = InitializeToolset
    $quietRestore = !$ci
    $testTargetFrameworks = if ($testCoreClr) { "netcoreapp2.1" } else { "" }

    # Do not set the property to true explicitly, since that would override value projects might set.
    $suppressExtensionDeployment = if (!$deployExtensions) { "/p:DeployExtension=false" } else { "" }

    MSBuild $toolsetBuildProj `
        $bl `
        /p:Configuration=$configuration `
        /p:Projects=$projects `
        /p:RepoRoot=$RepoRoot `
        /p:Restore=$restore `
        /p:Build=$build `
        /p:Rebuild=$rebuild `
        /p:Pack=$pack `
        /p:Sign=$sign `
        /p:Publish=$publish `
        /p:ContinuousIntegrationBuild=$ci `
        /p:OfficialBuildId=$officialBuildId `
        /p:BootstrapBuildPath=$bootstrapDir `
        /p:QuietRestore=$quietRestore `
        /p:QuietRestoreBinaryLog=$binaryLog `
        /p:TestTargetFrameworks=$testTargetFrameworks `
        $suppressExtensionDeployment `
        @properties
}

function TestAndAddToPath([string] $testPath) {
    if (Test-Path $testPath) {
        $env:PATH = "$testPath;$env:PATH"
        Write-Host "Added [$testPath] to the path."
    }
}

function UpdatePath() {
    # add highest framework dir
    $subdir = ""
    foreach ($child in Get-ChildItem "$env:WINDIR\Microsoft.NET\Framework\v4.0.?????") {
        $subdir = $child
    }
    TestAndAddToPath $subdir

    # add windows SDK dir for ildasm.exe
    foreach ($child in Get-ChildItem "${env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.?.? Tools") {
        $subdir = $child
    }
    TestAndAddToPath $subdir

    TestAndAddToPath "$ArtifactsDir\bin\fsc\$configuration\net472"
    TestAndAddToPath "$ArtifactsDir\bin\fsiAnyCpu\$configuration\net472"
}

function VerifyAssemblyVersions() {
    $fsiPath = Join-Path $ArtifactsDir "bin\fsi\Proto\net472\fsi.exe"

    # Only verify versions on CI or official build
    if ($ci -or $official) {
        $asmVerCheckPath = "$RepoRoot\scripts"
        Exec-Console $fsiPath """$asmVerCheckPath\AssemblyVersionCheck.fsx"" -- ""$ArtifactsDir"""
    }
}

function TestUsingNUnit([string] $testProject, [string] $targetFramework) {
    $dotnetPath = InitializeDotNetCli
    $dotnetExe = Join-Path $dotnetPath "dotnet.exe"
    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($testProject)
    $testLogPath = "$ArtifactsDir\TestResults\$configuration\${projectName}_$targetFramework.xml"
    $testBinLogPath = "$LogDir\${projectName}_$targetFramework.binlog"
    $args = "test $testProject --no-restore --no-build -c $configuration -f $targetFramework -v n --test-adapter-path . --logger ""nunit;LogFilePath=$testLogPath"" /bl:$testBinLogPath"
    Exec-Console $dotnetExe $args
}

function Prepare-TempDir() {
    Copy-Item (Join-Path $RepoRoot "tests\Resources\Directory.Build.props") $TempDir
    Copy-Item (Join-Path $RepoRoot "tests\Resources\Directory.Build.targets") $TempDir
}

try {
    Process-Arguments

    . (Join-Path $PSScriptRoot "build-utils.ps1")

    Update-Arguments

    Push-Location $RepoRoot

    if ($ci) {
        Prepare-TempDir
    }

    if ($bootstrap) {
        $bootstrapDir = Make-BootstrapBuild
    }

    if ($restore -or $build -or $rebuild -or $pack -or $sign -or $publish) {
        BuildSolution
    }

    if ($build) {
        VerifyAssemblyVersions
    }

    $desktopTargetFramework = "net472"
    $coreclrTargetFramework = "netcoreapp2.1"

    if ($testDesktop) {
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Compiler.UnitTests\FSharp.Compiler.UnitTests.fsproj" -targetFramework $desktopTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Build.UnitTests\FSharp.Build.UnitTests.fsproj" -targetFramework $desktopTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj" -targetFramework $desktopTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\tests\fsharp\FSharpSuite.Tests.fsproj" -targetFramework $desktopTargetFramework
    }

    if ($testCoreClr) {
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Compiler.UnitTests\FSharp.Compiler.UnitTests.fsproj" -targetFramework $coreclrTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Build.UnitTests\FSharp.Build.UnitTests.fsproj" -targetFramework $coreclrTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj" -targetFramework $coreclrTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\tests\fsharp\FSharpSuite.Tests.fsproj" -targetFramework $coreclrTargetFramework
    }

    if ($testFSharpQA) {
        Push-Location "$RepoRoot\tests\fsharpqa\source"
        $resultsRoot = "$ArtifactsDir\TestResults\$configuration"
        $resultsLog = "test-net40-fsharpqa-results.log"
        $errorLog = "test-net40-fsharpqa-errors.log"
        $failLog = "test-net40-fsharpqa-errors"
        $perlExe = "$env:USERPROFILE\.nuget\packages\StrawberryPerl64\5.22.2.1\Tools\perl\bin\perl.exe"
        Create-Directory $resultsRoot
        UpdatePath
        $env:HOSTED_COMPILER = 1
        $env:CSC_PIPE = "$env:USERPROFILE\.nuget\packages\Microsoft.Net.Compilers\2.7.0\tools\csc.exe"
        $env:FSCOREDLLPATH = "$ArtifactsDir\bin\fsc\$configuration\net472\FSharp.Core.dll"
        $env:LINK_EXE = "$RepoRoot\tests\fsharpqa\testenv\bin\link\link.exe"
        $env:OSARCH = $env:PROCESSOR_ARCHITECTURE
        Exec-Console $perlExe """$RepoRoot\tests\fsharpqa\testenv\bin\runall.pl"" -resultsroot ""$resultsRoot"" -results $resultsLog -log $errorLog -fail $failLog -cleanup:no -procs:$env:NUMBER_OF_PROCESSORS"
        Pop-Location
    }

    if ($testFSharpCore) {
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj" -targetFramework $desktopTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj" -targetFramework $coreclrTargetFramework
    }

    if ($testFSharpCompiler) {
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Compiler.UnitTests\FSharp.Compiler.UnitTests.fsproj" -targetFramework $desktopTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\tests\FSharp.Compiler.UnitTests\FSharp.Compiler.UnitTests.fsproj" -targetFramework $coreclrTargetFramework
    }

    if ($testVs) {
        TestUsingNUnit -testProject "$RepoRoot\vsintegration\tests\GetTypesVS.UnitTests\GetTypesVS.UnitTests.fsproj" -targetFramework $desktopTargetFramework
        TestUsingNUnit -testProject "$RepoRoot\vsintegration\tests\UnitTests\VisualFSharp.UnitTests.fsproj" -targetFramework $desktopTargetFramework
    }

    ExitWithExitCode 0
}
catch {
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    ExitWithExitCode 1
}
finally {
    Pop-Location
}
