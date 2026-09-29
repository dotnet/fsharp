param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [Parameter(Mandatory)][string] $Project,
    [Parameter(Mandatory)][string] $Framework,
    [Parameter(Mandatory)][string] $Id,
    [string[]] $Properties = @()
)

$ErrorActionPreference = 'Stop'
$Project = [IO.Path]::GetFullPath($Project)
$directory = Split-Path $Project
$output = Join-Path $PerformanceRoot "capture-$Id.json"
Push-Location $PerformanceRoot
try {
    & "$PerformanceRoot\sdk10\dotnet.exe" msbuild $Project -t:Build -p:Configuration=Release `
        "-p:TargetFramework=$Framework" -p:ProvideCommandLineArgs=true -p:SkipCompilerExecution=true `
        -p:NonExistentFile=force-performance-argument-capture -p:NuGetAudit=false `
        -getItem:FscCommandLineArgs,_ResolvedProjectReferencePaths -getProperty:TargetPath,TargetFramework `
        @Properties > $output
    if ($LASTEXITCODE -ne 0) { throw "Argument capture failed for $Project; see $output" }
} finally {
    Pop-Location
}
$captured = Get-Content $output -Raw | ConvertFrom-Json
$argv = @('fsc.dll')
foreach ($item in $captured.Items.FscCommandLineArgs) {
    $argument = $item.Identity
    if ($argument -eq '--times' -or $argument -like '--warnaserror*') { continue }
    if ($argument -match '\.(fs|fsi)$' -and $argument -notmatch '^-') {
        $argument = [IO.Path]::GetFullPath($argument.Trim('"'), $directory)
    } elseif ($argument -match '^(-r:|--reference:|--resource:|--keyfile:|--win32res:)([^,]+)(.*)$') {
        $argument = $Matches[1] + [IO.Path]::GetFullPath($Matches[2].Trim('"'), $directory) + $Matches[3]
    }
    $argv += $argument
}
if ($argv.Count -lt 3) { throw "No compiler arguments captured for $Project" }
if (-not ($argv | Where-Object { $_ -like '--langversion:*' })) { $argv += '--langversion:10.0' }
$argv += '--warnaserror-'
$references = @($captured.Items._ResolvedProjectReferencePaths | Where-Object {
    $_.MSBuildSourceProjectFile -like '*.fsproj'
} | ForEach-Object {
    $outputAssembly = if ($_.ReferenceAssembly) { $_.ReferenceAssembly } else { $_.Identity }
    if ($argv -notcontains "-r:$outputAssembly" -and $argv -notcontains "--reference:$outputAssembly") {
        throw "Project reference does not match compiler arguments: $outputAssembly"
    }
    [pscustomobject]@{path=$_.MSBuildSourceProjectFile; output=$outputAssembly; framework=$_.NearestTargetFramework}
})
[ordered]@{
    id=$Id
    project=$Project
    target_framework=$captured.Properties.TargetFramework
    output=$captured.Properties.TargetPath
    working_directory=$directory
    arguments=$argv
    references=$references
} | ConvertTo-Json -Depth 8 | Set-Content (Join-Path $PerformanceRoot "case-$Id.json")
Write-Output "$Id captured: $($argv.Count) arguments, $($references.Count) F# project references."
