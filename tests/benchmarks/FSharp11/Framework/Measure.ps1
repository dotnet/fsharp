param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [Parameter(Mandatory)][string] $Case,
    [ValidateSet('compile','check','calibrate')][string] $Operation = 'compile',
    [int] $StartRound = -2,
    [int] $EndRound = 12,
    [string[]] $Arms = @('old-framework','new-framework','old-net10','new-net10','old-net11','new-net11'),
    [string] $Cohort = 'measurements',
    [switch] $NoAllocation
)

$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath($PerformanceRoot)
$chains = & "$PSScriptRoot\Toolchains.ps1" -PerformanceRoot $root
$allArms = @($chains.Keys)
$casePath = "$root\case-$Case.json"
$directory = "$root\framework\$Cohort"
New-Item -ItemType Directory -Force $directory | Out-Null
for ($round = $StartRound; $round -lt $EndRound; $round++) {
    $shift = (($round % 6) + 6) % 6
    $order = @(0,1,5,2,4,3 | ForEach-Object {$allArms[($_ + $shift) % 6]} | Where-Object {$_ -in $Arms})
    if ($order.Count -ne $Arms.Count) { throw 'Unknown or duplicate arm.' }
    foreach ($arm in $order) {
        $tc = $chains[$arm]
        $id = "$Case-$Operation-$arm-$round"
        $resultPath = "$directory\$id.json"
        if (Test-Path $resultPath) { throw "Refusing to replace $id" }
        $info = [Diagnostics.ProcessStartInfo]::new($tc.host)
        $info.UseShellExecute = $false
        $info.RedirectStandardOutput = $true
        $info.RedirectStandardError = $true
        foreach ($key in @($info.Environment.Keys)) {
            if ($key -match '^(DOTNET_|COMPlus_)') { [void]$info.Environment.Remove($key) }
        }
        if ($tc.family -ne 'framework') {
            $info.Environment['DOTNET_ROOT'] = Split-Path $tc.host
            foreach ($arg in @('exec','--fx-version',$tc.runtime,'--roll-forward','Disable',$tc.collector)) {
                $info.ArgumentList.Add($arg)
            }
        }
        foreach ($arg in @($tc.payload,$casePath,$resultPath,$Operation)) { $info.ArgumentList.Add($arg) }
        if ($NoAllocation) { $info.ArgumentList.Add('no-allocation') }
        $timer = [Diagnostics.Stopwatch]::StartNew()
        $process = [Diagnostics.Process]::Start($info)
        $stdout = $process.StandardOutput.ReadToEndAsync()
        $stderr = $process.StandardError.ReadToEndAsync()
        if (-not $process.WaitForExit(1800000)) {
            $process.Kill($true); $process.WaitForExit()
            throw "Timed out: $id"
        }
        $timer.Stop()
        $stdout.GetAwaiter().GetResult() | Set-Content "$resultPath.stdout"
        $stderr.GetAwaiter().GetResult() | Set-Content "$resultPath.stderr"
        if (-not (Test-Path $resultPath)) { throw "Missing result: $id (exit $($process.ExitCode)); see $resultPath.stderr" }
        $result = Get-Content $resultPath -Raw | ConvertFrom-Json -AsHashtable
        $result['arm'] = $arm; $result['round'] = $round; $result['order'] = $order
        $result['warmup'] = $round -lt 0; $result['cohort'] = $Cohort
        $result['collector_sha256'] = (Get-FileHash $tc.collector).Hash
        $result['fcs_sha256'] = (Get-FileHash "$($tc.payload)\FSharp.Compiler.Service.dll").Hash
        $result['core_sha256'] = (Get-FileHash "$($tc.payload)\FSharp.Core.dll").Hash
        $result['controller_wall_ns'] = $timer.Elapsed.TotalNanoseconds
        $result['parent_exit_code'] = $process.ExitCode
        if ($Operation -eq 'compile' -and $result.status -eq 'accepted') {
            $inputCase = Get-Content $casePath -Raw | ConvertFrom-Json
            $outputs = @($inputCase.arguments | Where-Object {$_ -match '^(-o:|--out:)'})
            if ($outputs.Count -ne 1) { throw 'Expected one output for equivalence hashing.' }
            $emitted = [IO.Path]::GetFullPath(($outputs[0] -replace '^(-o:|--out:)', '').Trim('"'), $inputCase.working_directory)
            $result['emitted_sha256'] = (Get-FileHash $emitted).Hash
        }
        $process.Dispose()
        $result | ConvertTo-Json -Depth 12 | Set-Content $resultPath
        if ($result.status -ne 'accepted' -or $result.parent_exit_code -ne 0) { throw "Failed run: $id" }
        $runtime = if ($tc.family -eq 'framework') { ".NET Framework $($tc.runtime)" } else { ".NET $($tc.runtime)" }
        if ($result.runtime -ne $runtime -or $result.fcs_version -ne $tc.fcs_version) { throw "Wrong runtime/compiler: $id" }
        if ($result.fcs_target_framework -ne '.NETStandard,Version=v2.0' -or $result.core_target_framework -ne '.NETStandard,Version=v2.0') {
            throw "Incompatible payload: $id"
        }
        $allocation = if ($null -eq $result.allocated_bytes) { 'allocation not collected' }
            else { '{0:N3} GiB' -f ($result.allocated_bytes / 1GB) }
        Write-Output ('{0}: {1}, {2:N1} MiB peak, {3:N2}s' -f $id,
            $allocation, ($result.peak_working_set_bytes / 1MB), ($result.wall_ns / 1e9))
    }
}
