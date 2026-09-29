param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [switch] $ValidateOnly,
    [switch] $SkipBuild,
    [ValidateRange(0, 2)][int] $Launch = 0,
    [ValidateSet('sdk10', 'sdk11rc1')][string[]] $Arms = @('sdk10', 'sdk11rc1')
)

$ErrorActionPreference = 'Stop'
$PerformanceRoot = [IO.Path]::GetFullPath($PerformanceRoot)
$baseline = "$PerformanceRoot\sdk10"
$candidate = 'C:\Program Files\dotnet'
$runtime = '11.0.0-rc.1.26425.128'
$driver = "$PerformanceRoot\program-driver"
$source = Join-Path $PSScriptRoot 'Kernels.fs'
$toolchains = @{
    sdk10 = @{host="$baseline\dotnet.exe"; runtime='10.0.0'; sdk="$baseline\sdk\10.0.100\FSharp"}
    sdk11rc1 = @{host="$candidate\dotnet.exe"; runtime=$runtime; sdk="$candidate\sdk\11.0.100-rc.1.26425.128\FSharp"}
}
Push-Location $PerformanceRoot
try {
    if (-not $SkipBuild) {
        & "$baseline\dotnet.exe" build "$PSScriptRoot\Programs.csproj" -c Release -o $driver `
            "-p:BaseIntermediateOutputPath=$PerformanceRoot\program-driver-obj\" --nologo -v:q
        if ($LASTEXITCODE -ne 0) { throw 'Program driver build failed' }
    }
    foreach ($arm in $Arms) {
        $tc = $toolchains[$arm]
        $directory = "$PerformanceRoot\programs\$arm"
        New-Item -ItemType Directory -Force $directory | Out-Null
        $dll = "$directory\Kernels.dll"
        if (-not $SkipBuild) {
            $arguments = @('--target:library', '--targetprofile:netcore', '--noframework', '--simpleresolution',
                '--optimize+', '--debug:portable', '--deterministic+', '--langversion:10.0',
                "-o:$dll", "-r:$($tc.sdk)\FSharp.Core.dll")
            $arguments += @(Get-ChildItem "$baseline\packs\Microsoft.NETCore.App.Ref\10.0.0\ref\net10.0\*.dll" |
                Sort-Object Name | ForEach-Object { "-r:$($_.FullName)" })
            $arguments += $source
            $arguments | Set-Content "$directory\kernels.rsp"
            & $tc.host exec --fx-version $tc.runtime --roll-forward Disable "$($tc.sdk)\fsc.dll" "@$directory\kernels.rsp"
            if ($LASTEXITCODE -ne 0) { throw "Kernel compilation failed: $arm" }
            [ordered]@{toolchain=$arm; arguments=$arguments; source_sha256=(Get-FileHash $source).Hash} |
                ConvertTo-Json -Depth 4 | Set-Content "$directory\compilation.json"
        }
        $env:FSHARP_WORKLOAD_DLL = $dll
        $env:FSHARP_WORKLOAD_CORE = "$($tc.sdk)\FSharp.Core.dll"
        $env:FSHARP_PROGRAM_RESULTS = if ($ValidateOnly) { "$directory\validation" } else { "$directory\launch-$Launch" }
        if (-not $ValidateOnly -and (Test-Path "$env:FSHARP_PROGRAM_RESULTS\results\Programs-report-full.json")) {
            throw "Refusing to overwrite measured suite: $arm launch $Launch"
        }
        [string[]] $runArguments = if ($ValidateOnly) { @('--validate') } else { @('--filter', '*') }
        $logName = if ($ValidateOnly) { 'validation' } else { "benchmark-$Launch" }
        & "$candidate\dotnet.exe" exec --fx-version $runtime --roll-forward Disable "$driver\Programs.dll" @runArguments `
            *> "$directory\$logName.log"
        if ($LASTEXITCODE -ne 0) { throw "Program run failed: $arm; see $directory" }
        Write-Output "$arm completed on fixed runtime $runtime."
    }
} finally {
    Pop-Location
}
