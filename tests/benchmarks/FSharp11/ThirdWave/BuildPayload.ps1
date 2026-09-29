param([Parameter(Mandatory)][string] $PerformanceRoot)

$ErrorActionPreference = 'Stop'
$PerformanceRoot = [IO.Path]::GetFullPath($PerformanceRoot)
$source = "$PerformanceRoot\fsharp-rc2-source"
$dotnet = "$env:ProgramFiles\dotnet\dotnet.exe"
if (Test-Path "$PerformanceRoot\rc2-r2r") { throw 'A payload already exists. Preserve measured payloads; use a new preparation directory.' }
if (-not (Test-Path "$source\proto.proj")) { throw 'Extract the pinned VMR F# subtree into fsharp-rc2-source first.' }
& "$PSScriptRoot\VerifyPayload.ps1" -PerformanceRoot $PerformanceRoot -SourceOnly

function Invoke-Build([string] $Name, [string[]] $Arguments) {
    & $dotnet @Arguments "-bl:$PerformanceRoot\$Name.binlog" *> "$PerformanceRoot\$Name.log"
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $Name. Inspect $PerformanceRoot\$Name.log and its binlog." }
}

$common = @('-m:1', '-nr:false', '-p:BuildInParallel=false', '-p:IgnoreMibc=true',
    '-p:RestoreSources=https://www.nuget.org/api/v2', '-p:NuGetAudit=false', '-v:minimal')
$oldBuilding = $env:BUILDING_USING_DOTNET
$oldRoot = $env:DOTNET_ROOT
$oldPackages = $env:NUGET_PACKAGES
Push-Location $source
try {
    $env:DOTNET_ROOT = Split-Path $dotnet
    $env:NUGET_PACKAGES = 'C:\Nuget'
    $sdk = & $dotnet --version
    if ($LASTEXITCODE -ne 0 -or $sdk -ne '11.0.100-rc.1.26425.128') { throw "Unexpected bootstrap SDK: $sdk" }
    $env:BUILDING_USING_DOTNET = 'true'
    Invoke-Build 'rc2-bootstrap' (@('publish', '.\proto.proj', '-c', 'Proto',
        '-p:FSHARPCORE_USE_PACKAGE=true', '-p:PublishReadyToRun=false',
        '-p:DisableCompilerRedirection=true') + $common)

    $layout = "$source\Directory.Build.props.user"
    if ((Test-Path $layout) -and (Get-FileHash $layout).Hash -ne (Get-FileHash "$PSScriptRoot\BuildLayout.props").Hash) {
        throw 'An unexpected source-tree build overlay exists.'
    }
    Copy-Item "$PSScriptRoot\BuildLayout.props" $layout
    $env:BUILDING_USING_DOTNET = 'false'
    $release = @('-c', 'Release', '-p:DISABLE_ARCADE=true', '-p:FSHARPCORE_USE_PACKAGE=false',
        "-p:DotnetFscCompilerPath=$source\artifacts\Bootstrap\fsc\fsc.dll",
        "-p:FscToolPath=$env:ProgramFiles\dotnet", '-p:FscToolExe=dotnet.exe',
        '-p:SourceRevisionId=9cd6167a7265ce7264b22719503b7dfa9eb8f83c') + $common
    Invoke-Build 'rc2-release' (@('publish', '.\src\fsc\fscProject\fsc.fsproj', '-f', 'net11.0',
        '-p:PublishReadyToRun=false', '-o', "$PerformanceRoot\rc2-release-il") + $release)
    Invoke-Build 'rc2-core-net10' (@('build', '.\src\FSharp.Core\FSharp.Core.fsproj', '-f', 'net10.0') + $release)
    Invoke-Build 'rc2-r2r' (@('publish', "$PSScriptRoot\ReadyToRunPayload.csproj", '-c', 'Release',
        "-p:CompilerPayload=$PerformanceRoot\rc2-release-il",
        "-p:SdkCore=$source\artifacts\bin\FSharp.Core\Release\net10.0\FSharp.Core.dll",
        "-p:BaseIntermediateOutputPath=$PerformanceRoot\r2r-obj\",
        "-p:BaseOutputPath=$PerformanceRoot\r2r-bin\", '-o', "$PerformanceRoot\rc2-r2r") + $common)
} finally {
    Pop-Location
    $env:BUILDING_USING_DOTNET = $oldBuilding
    $env:DOTNET_ROOT = $oldRoot
    $env:NUGET_PACKAGES = $oldPackages
}
& "$PSScriptRoot\VerifyPayload.ps1" -PerformanceRoot $PerformanceRoot
