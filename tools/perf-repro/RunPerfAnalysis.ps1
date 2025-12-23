# Master orchestration script for F# compiler performance analysis
# This script runs the complete profiling workflow for issue #18807

param(
    [int]$Total = 1500,
    [int]$Methods = 10,
    [string]$Generated = "./generated",
    [string]$Results = "./results",
    [switch]$Help
)

# Helper functions for colored output
function Print-Header {
    param([string]$Message)
    Write-Host "========================================" -ForegroundColor Blue
    Write-Host $Message -ForegroundColor Blue
    Write-Host "========================================" -ForegroundColor Blue
}

function Print-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Print-Warning {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

function Print-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Print-Info {
    param([string]$Message)
    Write-Host "  $Message" -ForegroundColor White
}

# Show help
if ($Help) {
    Write-Host "Usage: .\RunPerfAnalysis.ps1 [options]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -Total <n>        Total number of Assert.Equal calls (default: 1500)"
    Write-Host "  -Methods <n>      Number of test methods (default: 10)"
    Write-Host "  -Generated <path> Directory for generated projects (default: ./generated)"
    Write-Host "  -Results <path>   Output directory for results (default: ./results)"
    Write-Host "  -Help             Show this help message"
    Write-Host ""
    Write-Host "Example:"
    Write-Host "  .\RunPerfAnalysis.ps1 -Total 1500 -Methods 10"
    exit 0
}

# Get script directory
$ScriptDir = $PSScriptRoot

# Display configuration
Print-Header "F# Compiler Performance Analysis"
Write-Host ""
Print-Info "Configuration:"
Print-Info "  Total Assert.Equal calls: $Total"
Print-Info "  Test methods: $Methods"
Print-Info "  Generated projects: $Generated"
Print-Info "  Results directory: $Results"
Write-Host ""

# Create directories
Print-Info "Creating directories..."
New-Item -ItemType Directory -Force -Path $Generated | Out-Null
New-Item -ItemType Directory -Force -Path $Results | Out-Null
Print-Success "Directories created"
Write-Host ""

# Step 1: Check prerequisites
Print-Header "Step 1: Checking Prerequisites"
Write-Host ""

# Check for dotnet
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Print-Error "dotnet CLI not found. Please install .NET SDK."
    exit 1
}
$dotnetVersion = (dotnet --version)
Print-Success "dotnet CLI found: $dotnetVersion"

# Check for dotnet-trace (optional)
if (Get-Command dotnet-trace -ErrorAction SilentlyContinue) {
    $traceVersion = (dotnet-trace --version | Select-Object -First 1)
    Print-Success "dotnet-trace found: $traceVersion"
} else {
    Print-Warning "dotnet-trace not found. Will use timing-only mode."
    Print-Info "To install: dotnet tool install -g dotnet-trace"
}
Write-Host ""

# Step 2: Run profiling workflow
Print-Header "Step 2: Running Profiling Workflow"
Write-Host ""

$StartTime = Get-Date

Print-Info "Executing ProfileCompilation.fsx..."
$profileScript = Join-Path $ScriptDir "ProfileCompilation.fsx"
$profileArgs = @(
    "fsi"
    "`"$profileScript`""
    "--total"
    $Total
    "--methods"
    $Methods
    "--generated"
    "`"$Generated`""
    "--output"
    "`"$Results`""
)

$profileCmd = "dotnet " + ($profileArgs -join " ")
Invoke-Expression $profileCmd

if ($LASTEXITCODE -ne 0) {
    Print-Error "Profiling failed"
    exit 1
}
Print-Success "Profiling completed successfully"

$EndTime = Get-Date
$Elapsed = ($EndTime - $StartTime).TotalSeconds

Write-Host ""
Print-Success "Profiling workflow completed in $([math]::Round($Elapsed, 2))s"
Write-Host ""

# Step 3: Analyze results and generate report
Print-Header "Step 3: Generating Analysis Report"
Write-Host ""

Print-Info "Executing AnalyzeTrace.fsx..."
$analyzeScript = Join-Path $ScriptDir "AnalyzeTrace.fsx"
$reportPath = Join-Path $Results "PERF_REPORT.md"
$analyzeArgs = @(
    "fsi"
    "`"$analyzeScript`""
    "--results"
    "`"$Results`""
    "--output"
    "`"$reportPath`""
)

$analyzeCmd = "dotnet " + ($analyzeArgs -join " ")
Invoke-Expression $analyzeCmd

if ($LASTEXITCODE -ne 0) {
    Print-Error "Report generation failed"
    exit 1
}
Print-Success "Report generated successfully"
Write-Host ""

# Step 4: Display summary
Print-Header "Step 4: Summary"
Write-Host ""

# Read and display summary
$summaryPath = Join-Path $Results "summary.txt"
if (Test-Path $summaryPath) {
    Get-Content $summaryPath
    Write-Host ""
}

# Final message
Print-Header "Analysis Complete!"
Write-Host ""
Print-Success "All steps completed successfully"
Print-Info "Results location: $Results"
Print-Info "Performance report: $reportPath"
Write-Host ""
Print-Info "To view the report:"
Print-Info "  Get-Content `"$reportPath`""
Print-Info "  # or open with your favorite markdown viewer"
Write-Host ""

# Optional: Display report preview
if (Test-Path $reportPath) {
    Print-Info "Report preview (first 50 lines):"
    Write-Host ""
    Get-Content $reportPath -Head 50
    Write-Host ""
    Print-Info "..."
    Print-Info "(see $reportPath for full report)"
}

Write-Host ""
Print-Success "Done!"
