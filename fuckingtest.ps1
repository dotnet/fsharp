# (A1) Build (Release) – create all artifacts needed by tests
.\Build.cmd -c Release

# (A2) Set up the VS experimental hive (same as CI step 'Setup VS Hive')
# This script will:
#   - Discover / validate a VS 17.x install
#   - Set / persist required hive registry + layout
#   - Emit the required environment variables in the current session
# If it has a -Configuration parameter in your branch, pass it; otherwise omit.
powershell -ExecutionPolicy Bypass -File .\eng\SetupVSHive.ps1 -Configuration Release

# (A3) (Optional) Verify env vars now exist
"`nVSAPPIDDIR=$env:VSAPPIDDIR"
"VS170COMNTOOLS=$env:VS170COMNTOOLS"

if (-not $env:VSAPPIDDIR -or -not $env:VS170COMNTOOLS) {
    Write-Host "Hive setup did not set required env vars. Aborting." -ForegroundColor Red
    exit 1
}

# (A4) Path to the unit test assembly
$testDll = Join-Path $PWD "artifacts\bin\VisualFSharp.UnitTests\Release\net472\VisualFSharp.UnitTests.dll"

if (-not (Test-Path $testDll)) {
    Write-Host "Test assembly missing: $testDll" -ForegroundColor Red
    exit 1
}

# (A5) Enable your instrumentation
$env:FSharpAutoImportDiag = "1"
$env:FSharpTargetsDiagnostic = "true"

# (A6) Run only the failing test using vstest.console (closer to CI than dotnet test for VS integration)
$vswhere = Join-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer" "vswhere.exe"
if (-not (Test-Path $vswhere)) {
    Write-Host "vswhere.exe not found; ensure VS 2022 installed." -ForegroundColor Yellow
}

# Optionally locate vstest.console if not on PATH
$vstest = Get-ChildItem -Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio" -Recurse -Include vstest.console.exe -ErrorAction SilentlyContinue |
          Where-Object { $_.FullName -like "*\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" } |
          Sort-Object Length | Select-Object -First 1

if (-not $vstest) {
    Write-Host "Could not locate vstest.console.exe; falling back to 'dotnet test'." -ForegroundColor Yellow
    dotnet test $testDll --framework net472 --filter "FullyQualifiedName=Tests.ProjectSystem.ProjectItems.RemoveAssemblyReference.NoIVsTrackProjectDocuments2Events"
    exit $LASTEXITCODE
}

& $vstest.FullName `
    $testDll `
    /Framework:".NETFramework,Version=v4.7.2" `
    /TestCaseFilter:"FullyQualifiedName=Tests.ProjectSystem.ProjectItems.RemoveAssemblyReference.NoIVsTrackProjectDocuments2Events" `
    /Logger:trx /InIsolation

Write-Host "`nDone."