param([Parameter(Mandatory)][string] $PerformanceRoot)

$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath($PerformanceRoot)
$output="$root\framework\frozen.json"
if(Test-Path $output){throw 'The prepared campaign is already frozen.'}
$files=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach($directory in 'old-payload','new-payload','collector-bin','program-bin','programs'){
    Get-ChildItem "$root\framework\$directory" -Recurse -File |
        Where-Object { $_.Extension -in '.dll','.exe','.config','.manifest' -or $_.Name -like '*.runtimeconfig.json' } |
        ForEach-Object { [void]$files.Add($_.FullName) }
}
foreach($path in @(
    "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\clr.dll",
    "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\clrjit.dll",
    "$root\sdk10\shared\Microsoft.NETCore.App\10.0.0\coreclr.dll",
    "$root\sdk10\shared\Microsoft.NETCore.App\10.0.0\clrjit.dll",
    "$env:ProgramFiles\dotnet\shared\Microsoft.NETCore.App\11.0.0-rc.1.26425.128\coreclr.dll",
    "$env:ProgramFiles\dotnet\shared\Microsoft.NETCore.App\11.0.0-rc.1.26425.128\clrjit.dll"
)){[void]$files.Add($path)}
$fingerprints=@($files|Sort-Object|ForEach-Object{
    $file=Get-Item -LiteralPath $_
    [ordered]@{path=$file.FullName;bytes=$file.Length;sha256=(Get-FileHash $file.FullName).Hash}
})
$registry=Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full'
[ordered]@{
    frozen_utc=[DateTimeOffset]::UtcNow;framework_release=$registry.Release;framework_version=$registry.Version
    files=$fingerprints
    calibrations=@(Get-ChildItem "$root\framework\calibration-frozen" -Filter '*.json'|
        ForEach-Object{Get-Content $_.FullName -Raw|ConvertFrom-Json})
    source_vmr='be46bdda4d6599b80dd4ccd89b2de49d96cbf36d'
    source_fsharp='9cd6167a7265ce7264b22719503b7dfa9eb8f83c'
    package_locks=@(Get-ChildItem $PSScriptRoot -Filter '*.packages.lock.json'|ForEach-Object{
        [ordered]@{name=$_.Name;sha256=(Get-FileHash $_.FullName).Hash}
    })
} | ConvertTo-Json -Depth 15 | Set-Content $output
Write-Output "Frozen $($fingerprints.Count) prepared binary/configuration files."
