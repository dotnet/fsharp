# Performance analysis orchestration script (Windows)
# Usage: .\RunPerfAnalysis.ps1 -Total 1500

param(
    [int]$Total = 1500,
    [int]$Methods = 10,
    [string]$Output = "./results",
    [switch]$Help
)

if ($Help) {
    Write-Host "Usage: .\RunPerfAnalysis.ps1 [-Total N] [-Methods N] [-Output DIR]"
    exit 0
}

$ScriptDir = $PSScriptRoot

Write-Host "=== F# Performance Analysis ===" -ForegroundColor Blue
Write-Host "Total: $Total, Methods: $Methods"

# Check dotnet
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: dotnet not found" -ForegroundColor Red
    exit 1
}

# Run profiler
Write-Host "`nRunning profiler..." -ForegroundColor Cyan
$profilerScript = Join-Path $ScriptDir "PerfProfiler.fsx"
dotnet fsi "$profilerScript" --total $Total --methods $Methods --output "$Output"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Profiling failed" -ForegroundColor Red
    exit 1
}

Write-Host "`nDone! Results in: $Output" -ForegroundColor Green
