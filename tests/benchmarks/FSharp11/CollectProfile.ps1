param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [Parameter(Mandatory)][string] $CasePath,
    [Parameter(Mandatory)][string] $Id,
    [Parameter(Mandatory)][ValidateSet('sdk10', 'sdk11rc1')][string] $Arm
)

$ErrorActionPreference = 'Stop'
$directory = Join-Path $PerformanceRoot 'profiles'
New-Item -ItemType Directory -Force $directory | Out-Null
$trace = Join-Path $directory "$Id.nettrace"
if (Test-Path $trace) { throw "Refusing to overwrite $trace" }
$hostPath = if ($Arm -eq 'sdk10') { "$PerformanceRoot\sdk10\dotnet.exe" } else { 'C:\Program Files\dotnet\dotnet.exe' }
$runtime = if ($Arm -eq 'sdk10') { '10.0.0' } else { '11.0.0-rc.1.26425.128' }
$sdk = if ($Arm -eq 'sdk10') {
    "$PerformanceRoot\sdk10\sdk\10.0.100\FSharp"
} else {
    'C:\Program Files\dotnet\sdk\11.0.100-rc.1.26425.128\FSharp'
}
Push-Location $PSScriptRoot
try {
    dotnet tool run dotnet-trace -- collect --providers 'Microsoft-Windows-DotNETRuntime:0x1280009:5' `
        --rundown false --buffersize 512 --output $trace -- $hostPath exec --fx-version $runtime --roll-forward Disable `
        "$PerformanceRoot\collector\Collector.dll" $sdk $CasePath "$directory\$Id-worker.json" compile `
        *> "$directory\$Id.log"
    if ($LASTEXITCODE -ne 0) { throw "Profile collection failed: $Id" }
    & 'C:\Program Files\dotnet\dotnet.exe' exec --fx-version 11.0.0-rc.1.26425.128 --roll-forward Disable `
        "$PerformanceRoot\profile-reader\Profile.dll" $trace $sdk "$directory\$Id-summary.json"
    if ($LASTEXITCODE -ne 0) { throw "Profile rejected: $Id" }
    $summary = Get-Content "$directory\$Id-summary.json" -Raw | ConvertFrom-Json -AsHashtable
    $summary['collection'] = @{
        id=$Id; toolchain=$Arm; runtime=$runtime; input_sha256=(Get-FileHash $CasePath).Hash
        provider='Microsoft-Windows-DotNETRuntime:0x1280009:5'; buffer_mib=512; rundown=$false
        startup_suspended=$true; tool_version='10.0.745401'; raw_trace=$trace
        reader_sha256=(Get-FileHash "$PerformanceRoot\profile-reader\Profile.dll").Hash
    }
    $summary | ConvertTo-Json -Depth 15 | Set-Content "$directory\$Id-summary.json"
    Write-Output "$Id accepted: $($summary.allocation_ticks) ticks, $($summary.events_lost) lost events."
} finally {
    Pop-Location
}
