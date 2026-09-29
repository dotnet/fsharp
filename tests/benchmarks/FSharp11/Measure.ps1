param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [Parameter(Mandatory)][string] $CasePath,
    [Parameter(Mandatory)][string] $ResultsDirectory,
    [ValidateSet('compile', 'check', 'calibrate')][string] $Operation = 'compile',
    [ValidateSet('shipping', 'compiler-on-runtime11', 'datas')][string] $Comparison = 'shipping',
    [int] $Pairs = 9,
    [int] $StartPair = 0,
    [int] $Warmups = 2,
    [int] $TimeoutSeconds = 1200
)

$ErrorActionPreference = 'Stop'
$PerformanceRoot = [IO.Path]::GetFullPath($PerformanceRoot)
$CasePath = [IO.Path]::GetFullPath($CasePath)
$ResultsDirectory = [IO.Path]::GetFullPath($ResultsDirectory)
New-Item -ItemType Directory -Force $ResultsDirectory | Out-Null
$case = Get-Content $CasePath -Raw | ConvertFrom-Json
$collector = Join-Path $PerformanceRoot 'collector\Collector.dll'
$collectorHash = (Get-FileHash $collector -Algorithm SHA256).Hash
$toolchains = @{
    sdk10 = @{
        host = Join-Path $PerformanceRoot 'sdk10\dotnet.exe'
        compiler = Join-Path $PerformanceRoot 'sdk10\sdk\10.0.100\FSharp'
        runtime = '10.0.0'
    }
    sdk11rc1 = @{
        host = 'C:\Program Files\dotnet\dotnet.exe'
        compiler = 'C:\Program Files\dotnet\sdk\11.0.100-rc.1.26425.128\FSharp'
        runtime = '11.0.0-rc.1.26425.128'
    }
}
if ($Comparison -eq 'compiler-on-runtime11') {
    $old = $toolchains.sdk10.Clone()
    $old.host = $toolchains.sdk11rc1.host
    $old.runtime = $toolchains.sdk11rc1.runtime
    $toolchains = @{'sdk10-on-runtime11'=$old; sdk11rc1=$toolchains.sdk11rc1}
} elseif ($Comparison -eq 'datas') {
    $off = $toolchains.sdk11rc1.Clone()
    $on = $toolchains.sdk11rc1.Clone()
    $off.datas = '0'
    $on.datas = '1'
    $toolchains = @{'sdk11rc1-datas-off'=$off; 'sdk11rc1-datas-on'=$on}
}
$arms = @($toolchains.Keys | Sort-Object)
$prefix = if ($Comparison -eq 'shipping') { $case.id } else { "$($case.id)-$Comparison" }

Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class ExperimentHost {
    [DllImport("kernel32.dll", SetLastError=true)]
    public static extern bool GetSystemTimes(out long idle, out long kernel, out long user);
}
'@

$inputFiles = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
$allArguments = @($case.arguments)
if ($Operation -eq 'check') {
    foreach ($project in $case.projects) { $allArguments += $project.arguments }
}
foreach ($argument in $allArguments) {
    $path = $null
    if ($argument -match '^(-r:|--reference:|--resource:|--keyfile:|--win32res:)(.*)$') {
        $path = ($Matches[2] -split ',')[0].Trim('"')
    } elseif ($argument -match '\.(fs|fsi)$') {
        $path = $argument.Trim('"')
    }
    if ($path) {
        if (-not [IO.Path]::IsPathRooted($path)) { $path = Join-Path $case.working_directory $path }
        [void]$inputFiles.Add([IO.Path]::GetFullPath($path))
    }
}
$inventory = @($inputFiles | Sort-Object | ForEach-Object {
    $file = Get-Item -LiteralPath $_
    [pscustomobject]@{path=$file.FullName; bytes=$file.Length; sha256=(Get-FileHash $file.FullName -Algorithm SHA256).Hash}
})
$inventoryPath = Join-Path $ResultsDirectory "$prefix-$Operation-inputs.json"
$inventory | ConvertTo-Json -Depth 5 | Set-Content $inventoryPath
$inventoryHash = (Get-FileHash $inventoryPath -Algorithm SHA256).Hash

for ($pair = $(if ($StartPair -eq 0) { -$Warmups } else { $StartPair }); $pair -lt $Pairs; $pair++) {
    $order = if (($pair % 2) -eq 0) { $arms } else { @($arms[1], $arms[0]) }
    foreach ($id in $order) {
        $tc = $toolchains[$id]
        $runId = "$prefix-$Operation-$id-$pair"
        $output = Join-Path $ResultsDirectory "$runId.json"
        if (Test-Path $output) { throw "Refusing to overwrite existing observation: $output" }
        $info = [Diagnostics.ProcessStartInfo]::new($tc.host)
        $info.UseShellExecute = $false
        $info.RedirectStandardOutput = $true
        $info.RedirectStandardError = $true
        foreach ($argument in @('exec', '--fx-version', $tc.runtime, '--roll-forward', 'Disable',
            $collector, $tc.compiler, $CasePath, $output, $Operation)) {
            $info.ArgumentList.Add($argument)
        }
        $info.Environment['DOTNET_CLI_TELEMETRY_OPTOUT'] = '1'
        $info.Environment.Remove('DOTNET_GCDynamicAdaptationMode') | Out-Null
        $info.Environment.Remove('COMPlus_GCDynamicAdaptationMode') | Out-Null
        if ($tc.ContainsKey('datas')) { $info.Environment['DOTNET_GCDynamicAdaptationMode'] = $tc.datas }
        $idle = $kernel = $user = 0L
        if (-not [ExperimentHost]::GetSystemTimes([ref]$idle, [ref]$kernel, [ref]$user)) { throw 'GetSystemTimes failed' }
        $startIdle = $idle
        $startTotal = $kernel + $user
        $timer = [Diagnostics.Stopwatch]::StartNew()
        $process = [Diagnostics.Process]::Start($info)
        $stdout = $process.StandardOutput.ReadToEndAsync()
        $stderr = $process.StandardError.ReadToEndAsync()
        if (-not $process.WaitForExit($TimeoutSeconds * 1000)) {
            $process.Kill($true)
            $process.WaitForExit()
            throw "Timed out: $runId"
        }
        $timer.Stop()
        $stdout.GetAwaiter().GetResult() | Set-Content "$output.stdout"
        $stderr.GetAwaiter().GetResult() | Set-Content "$output.stderr"
        if (-not [ExperimentHost]::GetSystemTimes([ref]$idle, [ref]$kernel, [ref]$user)) { throw 'GetSystemTimes failed' }
        if (-not (Test-Path $output)) { throw "No final snapshot: $runId (exit $($process.ExitCode)); see $output.stderr" }
        $result = Get-Content $output -Raw | ConvertFrom-Json -AsHashtable
        $cpuTicks = $process.TotalProcessorTime.Ticks
        $capacity = $kernel + $user - $startTotal
        $busy = $capacity - ($idle - $startIdle)
        $result['host_other_cpu_fraction'] = if ($capacity -gt 0) { [Math]::Max(0, ($busy - $cpuTicks) / $capacity) } else { $null }
        $result['run_id'] = $runId
        $result['toolchain'] = $id
        $result['comparison'] = $Comparison
        $result['pair'] = $pair
        $result['warmup'] = $pair -lt 0
        $result['collector_sha256'] = $collectorHash
        $result['input_inventory_sha256'] = $inventoryHash
        $result['controller_wall_ns'] = $timer.ElapsedTicks * (1e9 / [Diagnostics.Stopwatch]::Frequency)
        $result['process_cpu_ns'] = $cpuTicks * 100
        $result['parent_exit_code'] = $process.ExitCode
        $result['scope'] = if ($Operation -eq 'compile') { 'cold-fcs-compile' } else { $Operation }
        $result | ConvertTo-Json -Depth 10 | Set-Content $output
        $process.Dispose()
        if ($result.status -ne 'accepted' -or $result.parent_exit_code -ne 0) { throw "Failed observation: $runId; see $output.stderr" }
        Write-Output ("{0}: {1:N3} GiB allocated, {2:N1} MiB peak, {3:N2}s" -f
            $runId, ($result.allocated_bytes / 1GB), ($result.peak_working_set_bytes / 1MB), ($result.wall_ns / 1e9))
    }
}
