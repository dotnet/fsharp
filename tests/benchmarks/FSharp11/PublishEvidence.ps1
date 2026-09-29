param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [Parameter(Mandatory)][string] $Destination
)

$ErrorActionPreference = 'Stop'
$PerformanceRoot = [IO.Path]::GetFullPath($PerformanceRoot)
$Destination = [IO.Path]::GetFullPath($Destination)
$manifest = Get-Content "$Destination\experiment.json" -Raw | ConvertFrom-Json
$results = Get-Content "$Destination\results.json" -Raw | ConvertFrom-Json
$inputs = Join-Path $Destination 'inputs'
New-Item -ItemType Directory -Force $inputs | Out-Null
$ids = @($manifest.compilation.workloads.id) + @($manifest.ide.graphs | ForEach-Object { "$($_.workload)-ide" })
foreach ($id in $ids) {
    Copy-Item -LiteralPath "$PerformanceRoot\case-$id.json" -Destination $inputs
}
$inventoryFiles = Get-ChildItem "$PerformanceRoot\complete-input-measurements" -Filter '*-inputs.json'
$checkedFiles = [Collections.Generic.Dictionary[string, string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($inventory in $inventoryFiles) {
    foreach ($entry in (Get-Content $inventory.FullName -Raw | ConvertFrom-Json)) {
        if (-not $checkedFiles.ContainsKey($entry.path)) {
            $checkedFiles[$entry.path] = (Get-FileHash -LiteralPath $entry.path).Hash
        }
        if ($checkedFiles[$entry.path] -ne $entry.sha256) { throw "Frozen input changed: $($entry.path)" }
    }
    Copy-Item -LiteralPath $inventory.FullName -Destination $inputs
}
$toolchains = @{}
foreach ($tc in $manifest.toolchains) {
    $sdk = if ($tc.id -eq 'sdk10') {
        "$PerformanceRoot\sdk10\sdk\10.0.100\FSharp"
    } else {
        'C:\Program Files\dotnet\sdk\11.0.100-rc.1.26425.128\FSharp'
    }
    $hashes = @{}
    foreach ($entry in $tc.binary_sha256.PSObject.Properties) {
        $hash = (Get-FileHash -LiteralPath (Join-Path $sdk $entry.Name)).Hash.ToLowerInvariant()
        if ($hash -ne $entry.Value) { throw "SDK binary changed: $($tc.id) $($entry.Name)" }
        $hashes[$entry.Name] = $hash
    }
    $hostRoot = if ($tc.id -eq 'sdk10') { "$PerformanceRoot\sdk10" } else { 'C:\Program Files\dotnet' }
    $runtimeDirectory = "$hostRoot\shared\Microsoft.NETCore.App\$($tc.runtime)"
    $runtimeHashes = @{}
    foreach ($file in 'coreclr.dll', 'clrjit.dll', 'System.Private.CoreLib.dll') {
        $runtimeHashes[$file] = (Get-FileHash -LiteralPath "$runtimeDirectory\$file").Hash
    }
    $toolchains[$tc.id] = @{
        directory=$sdk; sha256=$hashes; host_sha256=(Get-FileHash "$hostRoot\dotnet.exe").Hash
        runtime_directory=$runtimeDirectory; runtime_sha256=$runtimeHashes
    }
}
$collectorHash = (Get-FileHash "$PerformanceRoot\collector\Collector.dll").Hash
$scalar = @($results.compilation) + @($results.ide) + @($results.controls)
if (@($scalar.collector_sha256 | Sort-Object -Unique).Count -ne 1 -or $scalar[0].collector_sha256 -ne $collectorHash) {
    throw 'Collector identity does not match the measured cohorts'
}
$calibrations = @{}
foreach ($arm in 'sdk10', 'sdk11rc1') {
    $calibrations[$arm] = Get-Content "$PerformanceRoot\calibration-$arm.json" -Raw | ConvertFrom-Json
}
$rejectedProfiles = @(
    @{file='fcs-sdk10-rejected-mixed-profile.nettrace'; reported_events_lost=-15258}
    @{file='fcs-sdk10-rejected-gc-rundown.nettrace'; reported_events_lost=-15338}
    @{file='fcs-sdk10.nettrace'; reported_events_lost=-15210}
    @{file='fcs-sdk10-runtime11.nettrace'; reported_events_lost=-14636}
    @{file='fsharp-compiler-service-allocation-sdk10.nettrace'; reported_events_lost=2147468923}
)
foreach ($profile in $rejectedProfiles) {
    $profile.path = Join-Path "$PerformanceRoot\profiles" $profile.file
    $profile.sha256 = (Get-FileHash -LiteralPath $profile.path).Hash
    $profile.reason = 'Did not pass the zero-loss integrity gate; invalid sequence/loss accounting is not interpreted as a measured object count.'
}
$graphValidation = @{}
foreach ($name in 'source-edge-sdk10', 'source-edge-sdk11rc1', 'missing-edge-result') {
    $graphValidation[$name] = Get-Content "$PerformanceRoot\ide-validation\$name.json" -Raw | ConvertFrom-Json
}
$harnessFiles = @(Get-ChildItem $PSScriptRoot -Recurse -File -Force | Sort-Object FullName | ForEach-Object {
    [ordered]@{file=[IO.Path]::GetRelativePath($PSScriptRoot, $_.FullName); sha256=(Get-FileHash $_.FullName).Hash}
})
Copy-Item -LiteralPath "$PerformanceRoot\cli-validation\equivalence.json" -Destination "$Destination\cli-equivalence.json"
[ordered]@{
    schema_version=1
    generated_utc=[DateTimeOffset]::UtcNow.ToString('O')
    artifact_directory=$PerformanceRoot
    toolchains=$toolchains
    collector_sha256=$collectorHash
    program_driver_sha256=(Get-FileHash "$PerformanceRoot\program-driver\Programs.dll").Hash
    profile_reader_sha256=(Get-FileHash "$PerformanceRoot\profile-reader\Profile.dll").Hash
    harness_files=$harnessFiles
    checked_unique_input_files=$checkedFiles.Count
    input_manifests=@(Get-ChildItem $inputs -File | Sort-Object Name | ForEach-Object {
        [ordered]@{file=$_.Name; sha256=(Get-FileHash $_.FullName).Hash}
    })
    calibration=$calibrations
    source_graph_validation=$graphValidation
    rejected_profiles=$rejectedProfiles
    superseded_program_suites=@{
        reason='Pilot suites had shorter warmup/measurement counts; only launch-0/1/2 suites enter the published data.'
        paths=@("$PerformanceRoot\programs\sdk10\pilot-results", "$PerformanceRoot\programs\sdk10\results", "$PerformanceRoot\programs\sdk11rc1\results")
    }
    compilation_observations=$results.compilation.Count
    ide_observations=$results.ide.Count
    control_observations=$results.controls.Count
    generated_program_launch_cases=$results.generated_programs.Count
} | ConvertTo-Json -Depth 20 | Set-Content "$Destination\provenance.json"
Write-Output "Published evidence; $($checkedFiles.Count) unique input files still match their frozen hashes."
