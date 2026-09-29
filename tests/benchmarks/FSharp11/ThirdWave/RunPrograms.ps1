param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [switch] $ValidateOnly,
    [switch] $SkipBuild,
    [ValidateRange(0, 2)][int] $Launch = 0,
    [ValidateSet('sdk10', 'sdk11rc1', 'vmr-rc2-r2r')]
    [string[]] $Arms = @('sdk10', 'sdk11rc1', 'vmr-rc2-r2r')
)

$ErrorActionPreference = 'Stop'
$PerformanceRoot = [IO.Path]::GetFullPath($PerformanceRoot)
$source = Join-Path (Split-Path $PSScriptRoot) 'Kernels.fs'
$driver = "$PerformanceRoot\program-driver\Programs.dll"
$original = Get-Content "$PerformanceRoot\programs\sdk10\launch-0\provenance.json" -Raw | ConvertFrom-Json
if ((Get-FileHash $driver).Hash -ne $original.driver_sha256) { throw 'The fixed program driver changed.' }
$toolchains = & "$PSScriptRoot\Toolchains.ps1" -PerformanceRoot $PerformanceRoot
foreach ($arm in $Arms) {
    $tc = $toolchains[$arm]
    $directory = "$PerformanceRoot\third-wave\programs\$arm"
    New-Item -ItemType Directory -Force $directory | Out-Null
    $dll = "$directory\Kernels.dll"
    if (-not $SkipBuild) {
        if (Test-Path $dll) { throw "Refusing to replace compiled benchmark: $dll" }
        $arguments = @('--target:library', '--targetprofile:netcore', '--noframework', '--simpleresolution',
            '--optimize+', '--debug:portable', '--deterministic+', "-o:$dll", "-r:$($tc.compiler)\FSharp.Core.dll")
        $arguments += @(Get-ChildItem "$PerformanceRoot\sdk10\packs\Microsoft.NETCore.App.Ref\10.0.0\ref\net10.0\*.dll" |
            Sort-Object Name | ForEach-Object { "-r:$($_.FullName)" })
        $arguments += $source
        $arguments | Set-Content "$directory\kernels.rsp"
        & $tc.host exec --fx-version $tc.runtime --roll-forward Disable "$($tc.compiler)\fsc.dll" "@$directory\kernels.rsp"
        if ($LASTEXITCODE -ne 0) { throw "Kernel compilation failed: $arm" }
        [ordered]@{toolchain=$arm; arguments=$arguments; language='compiler default'; source_sha256=(Get-FileHash $source).Hash} |
            ConvertTo-Json -Depth 4 | Set-Content "$directory\compilation.json"
    }
    $output = if ($ValidateOnly) { "$directory\validation" } else { "$directory\launch-$Launch" }
    if (Test-Path "$output\provenance.json") { throw "Refusing to replace suite evidence: $output" }
    $info = [Diagnostics.ProcessStartInfo]::new($tc.host)
    $info.UseShellExecute = $false
    $info.RedirectStandardOutput = $true
    $info.RedirectStandardError = $true
    foreach ($key in @($info.Environment.Keys)) {
        if ($key -match '^(DOTNET_|COMPlus_)') { [void]$info.Environment.Remove($key) }
    }
    $info.Environment['DOTNET_ROOT'] = Split-Path $tc.host
    $info.Environment['DOTNET_CLI_TELEMETRY_OPTOUT'] = '1'
    $info.Environment['FSHARP_WORKLOAD_DLL'] = $dll
    $info.Environment['FSHARP_WORKLOAD_CORE'] = "$($tc.compiler)\FSharp.Core.dll"
    $info.Environment['FSHARP_PROGRAM_RESULTS'] = $output
    foreach ($arg in @('exec', '--fx-version', $tc.runtime, '--roll-forward', 'Disable', $driver)) {
        $info.ArgumentList.Add($arg)
    }
    if ($ValidateOnly) { $info.ArgumentList.Add('--validate') }
    else { $info.ArgumentList.Add('--filter'); $info.ArgumentList.Add('*') }
    $process = [Diagnostics.Process]::Start($info)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit(3600000)) {
        $process.Kill($true)
        $process.WaitForExit()
        throw "Program suite timed out: $arm"
    }
    $stdout.GetAwaiter().GetResult() | Set-Content "$output.stdout"
    $stderr.GetAwaiter().GetResult() | Set-Content "$output.stderr"
    $exitCode = $process.ExitCode
    $process.Dispose()
    if ($exitCode -ne 0) { throw "Program suite failed: $arm; see $output.stderr and $output.stdout" }
    $provenance = Get-Content "$output\provenance.json" -Raw | ConvertFrom-Json
    if ($provenance.runtime -ne ".NET $($tc.runtime)") { throw "Wrong program runtime: $arm" }
    Write-Output "$arm completed on its assigned runtime $($tc.runtime)."
}
