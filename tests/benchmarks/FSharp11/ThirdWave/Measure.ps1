param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [Parameter(Mandatory)][string] $CasePath,
    [ValidateSet('compile', 'check')][string] $Operation = 'compile',
    [int] $StartRound = -2,
    [int] $EndRound = 12,
    [int] $TimeoutSeconds = 1200
)

$ErrorActionPreference = 'Stop'
$PerformanceRoot = [IO.Path]::GetFullPath($PerformanceRoot)
$CasePath = [IO.Path]::GetFullPath($CasePath)
$case = Get-Content $CasePath -Raw | ConvertFrom-Json
$destination = Join-Path $PerformanceRoot 'third-wave\measurements'
New-Item -ItemType Directory -Force $destination | Out-Null
$collector = Join-Path $PerformanceRoot 'collector\Collector.dll'
$collectorHash = (Get-FileHash $collector).Hash
if ($collectorHash -ne '449F9E2C9B5BD00A64B259082994945F43880D9E2E9844B164F45278576290B1') {
    throw 'The collector differs from the verified first-campaign binary.'
}
$toolchains = & "$PSScriptRoot\Toolchains.ps1" -PerformanceRoot $PerformanceRoot
$orders = @(
    @('sdk10', 'sdk11rc1', 'vmr-rc2-r2r'),
    @('sdk11rc1', 'vmr-rc2-r2r', 'sdk10'),
    @('vmr-rc2-r2r', 'sdk10', 'sdk11rc1'),
    @('vmr-rc2-r2r', 'sdk11rc1', 'sdk10'),
    @('sdk11rc1', 'sdk10', 'vmr-rc2-r2r'),
    @('sdk10', 'vmr-rc2-r2r', 'sdk11rc1')
)

for ($round = $StartRound; $round -lt $EndRound; $round++) {
    $order = $orders[(($round % 6) + 6) % 6]
    foreach ($arm in $order) {
        $tc = $toolchains[$arm]
        $runId = "$($case.id)-$Operation-$arm-$round"
        $output = Join-Path $destination "$runId.json"
        if (Test-Path $output) { throw "Refusing to overwrite observation: $output" }
        $info = [Diagnostics.ProcessStartInfo]::new($tc.host)
        $info.UseShellExecute = $false
        $info.RedirectStandardOutput = $true
        $info.RedirectStandardError = $true
        foreach ($key in @($info.Environment.Keys)) {
            if ($key -match '^(DOTNET_|COMPlus_)') { [void]$info.Environment.Remove($key) }
        }
        $info.Environment['DOTNET_ROOT'] = Split-Path $tc.host
        $info.Environment['DOTNET_CLI_TELEMETRY_OPTOUT'] = '1'
        foreach ($arg in @('exec', '--fx-version', $tc.runtime, '--roll-forward', 'Disable',
                $collector, $tc.compiler, $CasePath, $output, $Operation)) {
            $info.ArgumentList.Add($arg)
        }
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
        if (-not (Test-Path $output)) { throw "No observation: $runId; see $output.stderr" }
        $result = Get-Content $output -Raw | ConvertFrom-Json -AsHashtable
        $result['campaign'] = 'third-wave'
        $result['run_id'] = $runId
        $result['toolchain'] = $arm
        $result['round'] = $round
        $result['order'] = $order
        $result['warmup'] = $round -lt 0
        $result['collector_sha256'] = $collectorHash
        $result['controller_wall_ns'] = $timer.Elapsed.TotalNanoseconds
        $result['process_cpu_ns'] = $process.TotalProcessorTime.TotalNanoseconds
        $result['parent_exit_code'] = $process.ExitCode
        $result | ConvertTo-Json -Depth 12 | Set-Content $output
        $process.Dispose()
        if ($result.status -ne 'accepted' -or $result.parent_exit_code -ne 0) {
            throw "Failed observation: $runId; see $output.stderr"
        }
        if ($result.runtime -ne ".NET $($tc.runtime)" -or $result.fcs_version -ne $tc.version) {
            throw "Unexpected runtime or compiler identity: $runId"
        }
        Write-Output ('{0}: {1:N3} GiB allocated, {2:N1} MiB peak, {3:N2}s' -f
            $runId, ($result.allocated_bytes / 1GB), ($result.peak_working_set_bytes / 1MB), ($result.wall_ns / 1e9))
    }
}
