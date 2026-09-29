param(
    [Parameter(Mandatory)][string] $PerformanceRoot,
    [Parameter(Mandatory)][string] $Project,
    [Parameter(Mandatory)][string] $Framework,
    [Parameter(Mandatory)][string] $Id,
    [string[]] $Properties = @()
)

$ErrorActionPreference = 'Stop'
$nodes = [Collections.Generic.List[object]]::new()
$visited = @{}
function Visit([string] $file, [string] $tfm) {
    $file = [IO.Path]::GetFullPath($file)
    if ($visited.ContainsKey($file)) {
        if ($visited[$file] -ne $tfm) { throw "Multiple TFMs for the same graph node: $file" }
        return
    }
    $visited[$file] = $tfm
    $captureId = "$Id-$([IO.Path]::GetFileNameWithoutExtension($file))-$tfm"
    & "$PSScriptRoot\Capture.ps1" -PerformanceRoot $PerformanceRoot -Project $file `
        -Framework $tfm -Id $captureId -Properties $Properties
    $case = Get-Content (Join-Path $PerformanceRoot "case-$captureId.json") -Raw | ConvertFrom-Json
    foreach ($reference in $case.references) {
        if (-not $reference.framework) { throw "No evaluated reference framework: $($reference.path)" }
        Visit $reference.path $reference.framework
    }
    $nodes.Add([ordered]@{
        path=$file; framework=$tfm; output=$case.output
        arguments=@($case.arguments | Select-Object -Skip 1)
        references=$case.references
    })
}
Visit $Project $Framework
[ordered]@{
    id=$Id; working_directory=(Split-Path ([IO.Path]::GetFullPath($Project)))
    arguments=@(); projects=$nodes.ToArray()
} | ConvertTo-Json -Depth 10 | Set-Content (Join-Path $PerformanceRoot "case-$Id.json")
Write-Output "$Id graph: $($nodes.Count) unique, topologically ordered project/TFM nodes."
