# scripts/local-validate.ps1
# D10 / G7: Run G1+G2+G3 locally before pushing.
# Exits 0 if all gates pass; non-zero with diagnostics if any gate fails.
# Per Constraint 9 in plan.md: contributors run this before every push.

[CmdletBinding()]
param(
  [string]$Configuration = 'Debug'
)
Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path "$PSScriptRoot\..").Path

function Step($name, [scriptblock]$action) {
  Write-Host ""
  Write-Host "==========================================================="
  Write-Host "  $name"
  Write-Host "==========================================================="
  $sw = [Diagnostics.Stopwatch]::StartNew()
  try {
    & $action
    $sw.Stop()
    Write-Host "[$name] OK ($([int]$sw.Elapsed.TotalSeconds)s)"
  } catch {
    $sw.Stop()
    Write-Host "[$name] FAIL ($([int]$sw.Elapsed.TotalSeconds)s): $_"
    throw
  }
}

# G1 — build-script smoke
Step "G1: build.cmd net40 $Configuration (compiler-only smoke)" {
  Push-Location $repoRoot
  try {
    $env:FSC_BUILD_SETUP = '0'
    & cmd.exe /c "build.cmd net40 $Configuration"
    if ($LASTEXITCODE -ne 0) { throw "build.cmd exited $LASTEXITCODE" }
    $fsc = Join-Path $repoRoot "$Configuration\net40\bin\fsc.exe"
    if (-not (Test-Path $fsc)) { throw "fsc.exe not produced at $fsc" }
    $help = & $fsc --help | Select-Object -First 5
    Write-Host ($help -join "`n")
  } finally { Pop-Location }
}

# G2 — bootstrap (proto-fsc + real-fsc + FSharp.Core)
Step "G2: proto-fsc -> real-fsc -> FSharp.Core" {
  Push-Location $repoRoot
  try {
    $msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -version "[15.0,16.0)" -find "MSBuild\15.0\bin\MSBuild.exe" | Select-Object -First 1
    if (-not $msbuild) { throw "MSBuild 15 not found via vswhere [15.0,16.0); install VS 2017 build tools" }
    Write-Host "Using MSBuild: $msbuild"

    & $msbuild src\fsharp-proto-build.proj /t:Build /p:Configuration=Proto /p:BUILD_PROTO_WITH_CORECLR_LKG=1 /p:DisableLocalization=true /bl:proto.binlog
    if ($LASTEXITCODE -ne 0) { throw "proto build failed" }

    & $msbuild build-everything.proj /t:Build /p:Configuration=Release /p:BUILD_NET40_FSHARP_CORE=1 /p:BUILD_NET40=1 /bl:real.binlog
    if ($LASTEXITCODE -ne 0) { throw "real build failed" }

    foreach ($f in @('Proto\net40\bin\fsc.exe','Release\net40\bin\fsc.exe','Release\net40\bin\FSharp.Core.dll')) {
      $p = Join-Path $repoRoot $f
      if (-not (Test-Path $p)) { throw "expected output missing: $f" }
    }
    $coreVersion = [System.Reflection.AssemblyName]::GetAssemblyName((Join-Path $repoRoot 'Release\net40\bin\FSharp.Core.dll')).Version.ToString()
    Write-Host "FSharp.Core.dll AssemblyVersion: $coreVersion"
    if ($coreVersion -ne '4.5.0.0') {
      throw "FSharp.Core.dll AssemblyVersion is $coreVersion; expected 4.5.0.0 (FSCoreVersion in build/targets/AssemblyVersions.props)"
    }
  } finally { Pop-Location }
}

# G3 — functional smoke
Step "G3: fsi/fsc/hello.exe + records/DUs/CEs + #r + .fsi consumption" {
  $bin = Join-Path $repoRoot 'Release\net40\bin'
  Push-Location $bin
  try {
    # 3.1 FSI sanity (use 'empty.fsx', NOT 'NUL.fsx' — NUL is reserved on Windows)
    Set-Content -Path empty.fsx -Value '' -NoNewline
    & .\fsi.exe --exec --use:empty.fsx
    if ($LASTEXITCODE -ne 0) { throw "fsi --exec --use:empty.fsx exited $LASTEXITCODE" }

    # 3.2 FSC produces a runnable PE
    Set-Content -Path hello.fs -Value 'printfn "hello"'
    & .\fsc.exe -o:hello.exe hello.fs
    if ($LASTEXITCODE -ne 0) { throw "fsc -o:hello.exe failed" }
    $out = & .\hello.exe
    if ($LASTEXITCODE -ne 0 -or $out -ne 'hello') { throw "hello.exe printed '$out' exit $LASTEXITCODE; expected 'hello'" }

    # 3.3 Records / DUs
    Set-Content -Path t.fsx -Value @'
type R = { x:int; y:string }
type Color = Red | Green | Blue
let r = { x = 1; y = "two" }
printfn "%A %A" r Red
'@
    & .\fsi.exe --exec --use:t.fsx
    if ($LASTEXITCODE -ne 0) { throw "records/DUs t.fsx failed" }

    # 3.4 #r assembly load
    Set-Content -Path rtest.fsx -Value @'
#r "System.Xml"
printfn "%A" typeof<System.Xml.XmlDocument>
'@
    & .\fsi.exe --exec --use:rtest.fsx
    if ($LASTEXITCODE -ne 0) { throw "#r rtest.fsx failed" }

    # 3.5 .fsi signature consumption
    Set-Content -Path lib.fsi -Value @'
namespace L
module M =
    val add : int -> int -> int
'@
    Set-Content -Path lib.fs -Value @'
namespace L
module M =
    let add a b = a + b
'@
    & .\fsc.exe -a -o:lib.dll lib.fsi lib.fs
    if ($LASTEXITCODE -ne 0) { throw "fsc .fsi+.fs compile failed" }

    # Cleanup smoke artifacts
    Remove-Item -Force empty.fsx,hello.fs,hello.exe,t.fsx,rtest.fsx,lib.fsi,lib.fs,lib.dll -ErrorAction SilentlyContinue
  } finally { Pop-Location }
}

Write-Host ""
Write-Host "==========================================================="
Write-Host "  ALL GATES PASSED — safe to push"
Write-Host "==========================================================="
