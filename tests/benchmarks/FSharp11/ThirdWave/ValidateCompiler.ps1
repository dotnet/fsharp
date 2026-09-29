param([Parameter(Mandatory)][string] $PerformanceRoot)

$ErrorActionPreference = 'Stop'
$PerformanceRoot = [IO.Path]::GetFullPath($PerformanceRoot)
$tc = (& "$PSScriptRoot\Toolchains.ps1" -PerformanceRoot $PerformanceRoot)['vmr-rc2-r2r']
$directory = "$PerformanceRoot\third-wave\cli"
New-Item -ItemType Directory -Force $directory | Out-Null
$records = foreach ($id in 'fsharp-core', 'fsharp-compiler-service', 'fstoolkit', 'oxpecker', 'nu', 'fsautocomplete') {
    $casePath = "$PerformanceRoot\case-$id.json"
    $case = Get-Content $casePath -Raw | ConvertFrom-Json
    $outputArgument = @($case.arguments | Where-Object { $_ -match '^(-o:|--out:)' })
    if ($outputArgument.Count -ne 1) { throw "Expected one output: $id" }
    $output = [IO.Path]::GetFullPath(($outputArgument[0] -replace '^(-o:|--out:)', '').Trim('"'), $case.working_directory)
    if (Test-Path "$directory\$id-fcs.json") { throw "Existing CLI validation: $id" }
    & $tc.host exec --fx-version $tc.runtime --roll-forward Disable "$PerformanceRoot\collector\Collector.dll" `
        $tc.compiler $casePath "$directory\$id-fcs.json" compile *> "$directory\$id-fcs.log"
    if ($LASTEXITCODE -ne 0) { throw "FCS validation failed: $id" }
    $fcsHash = (Get-FileHash $output).Hash
    Copy-Item $output "$directory\$id-fcs.dll"
    $arguments = @($case.arguments | Select-Object -Skip 1)
    if ($arguments | Where-Object { $_ -match '[\r\n]' }) { throw "Multiline compiler argument: $id" }
    $arguments | Set-Content "$directory\$id.rsp"
    Push-Location $case.working_directory
    try {
        & $tc.host exec --fx-version $tc.runtime --roll-forward Disable "$($tc.compiler)\fsc.dll" `
            "@$directory\$id.rsp" *> "$directory\$id-cli.log"
        if ($LASTEXITCODE -ne 0) { throw "CLI validation failed: $id" }
    } finally { Pop-Location }
    $cliHash = (Get-FileHash $output).Hash
    $record = [ordered]@{
        case=$id; toolchain='vmr-rc2-r2r'; input_sha256=(Get-FileHash $casePath).Hash
        response_sha256=(Get-FileHash "$directory\$id.rsp").Hash
        fcs_output_sha256=$fcsHash; cli_output_sha256=$cliHash; identical=$fcsHash -eq $cliHash
    }
    $record | ConvertTo-Json | Set-Content "$directory\$id-equivalence.json"
    if (-not $record.identical) { throw "FCS and CLI differ: $id. Both evidence and FCS output are retained." }
    Remove-Item -LiteralPath "$directory\$id-fcs.dll"
    $record
}
@($records) | ConvertTo-Json -Depth 5 | Set-Content "$directory\equivalence.json"

$traceDirectory = "$PerformanceRoot\third-wave\profile"
New-Item -ItemType Directory -Force $traceDirectory | Out-Null
$trace = "$traceDirectory\oxpecker-rc2.nettrace"
if (Test-Path $trace) { throw 'A third-wave profile already exists.' }
Push-Location (Split-Path $PSScriptRoot)
try {
    & $tc.host tool run dotnet-trace -- collect --providers 'Microsoft-Windows-DotNETRuntime:0x1280009:5' `
        --rundown false --buffersize 512 --output $trace -- $tc.host exec --fx-version $tc.runtime --roll-forward Disable `
        "$PerformanceRoot\collector\Collector.dll" $tc.compiler "$PerformanceRoot\case-oxpecker.json" `
        "$traceDirectory\worker.json" compile *> "$traceDirectory\collection.log"
    if ($LASTEXITCODE -ne 0) { throw 'Allocation profile collection failed.' }
    & $tc.host exec --fx-version $tc.runtime --roll-forward Disable "$PerformanceRoot\profile-reader\Profile.dll" `
        $trace $tc.compiler "$traceDirectory\summary.json"
    if ($LASTEXITCODE -ne 0) { throw 'Allocation profile rejected; preserve the trace and inspect its integrity.' }
} finally { Pop-Location }
$summary = Get-Content "$traceDirectory\summary.json" -Raw | ConvertFrom-Json -AsHashtable
$summary['collection'] = @{
    toolchain='vmr-rc2-r2r'; runtime=$tc.runtime; input_sha256=(Get-FileHash "$PerformanceRoot\case-oxpecker.json").Hash
    provider='Microsoft-Windows-DotNETRuntime:0x1280009:5'; buffer_mib=512; rundown=$false; startup_suspended=$true
    tool_version='10.0.745401'; raw_trace=$trace; raw_sha256=(Get-FileHash $trace).Hash
    reader_sha256=(Get-FileHash "$PerformanceRoot\profile-reader\Profile.dll").Hash
}
$summary | ConvertTo-Json -Depth 15 | Set-Content "$traceDirectory\summary.json"
Write-Output "Six identical FCS/CLI outputs; accepted profile with $($summary.events_lost) lost events."
