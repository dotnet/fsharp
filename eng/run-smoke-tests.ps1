<#
  F# 15.9 build-output smoke tests.

  Validates the actual shipped compiler artifacts (fsc.exe / fsi.exe from the product build) by
  compiling and executing real F# code. This is the first, fast test gate that runs inside CI after
  the product build, so a green pipeline means the compiler it produced can actually compile and run
  F# - not merely that the build completed. Heavier unit/suite tests layer on top of this.

  Exits non-zero on any failure so CI fails loudly.
#>
[CmdletBinding()]
param(
    [string]$BinDir,
    [string]$WorkDir = (Join-Path $env:TEMP ("fsharp-smoke-" + [System.Guid]::NewGuid().ToString('N')))
)

$ErrorActionPreference = 'Stop'

if (-not $BinDir) {
    $scriptDir = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
    $BinDir = Join-Path $scriptDir '..\release\net40\bin'
}
$fsc = Join-Path $BinDir 'fsc.exe'
$fsi = Join-Path $BinDir 'fsi.exe'

function Fail($msg) { Write-Host "SMOKE FAIL: $msg" -ForegroundColor Red; exit 1 }

if (-not (Test-Path $fsc)) { Fail "fsc.exe not found at $fsc (product build did not run?)" }
if (-not (Test-Path $fsi)) { Fail "fsi.exe not found at $fsi (product build did not run?)" }

New-Item -ItemType Directory -Force -Path $WorkDir | Out-Null
Write-Host "Smoke tests using:`n  fsc = $fsc`n  fsi = $fsi`n  work = $WorkDir"

$failures = 0

# 1) fsc: compile an exe that exercises FSharp.Core (List/Seq/Async) and run it.
$src = Join-Path $WorkDir 'smoke.fs'
@'
module Smoke
[<EntryPoint>]
let main _ =
    let listSum = List.sum [1..100]
    let seqLen = Seq.length (seq { for i in 1..50 -> i * i })
    let asyncVal = Async.RunSynchronously (async { return listSum + seqLen })
    printfn "SMOKE_OK %d" asyncVal
    if asyncVal = 5050 + 50 then 0 else 1
'@ | Set-Content $src -Encoding UTF8

$exe = Join-Path $WorkDir 'smoke.exe'
& $fsc --nologo --target:exe --out:$exe $src
if ($LASTEXITCODE -ne 0) { Write-Host "SMOKE FAIL: fsc failed to compile smoke.fs"; $failures++ }
elseif (-not (Test-Path $exe)) { Write-Host "SMOKE FAIL: fsc produced no exe"; $failures++ }
else {
    $out = & $exe
    if ($LASTEXITCODE -ne 0 -or ($out -notmatch 'SMOKE_OK 5100')) { Write-Host "SMOKE FAIL: compiled exe wrong result: $out (exit $LASTEXITCODE)"; $failures++ }
    else { Write-Host "  [PASS] fsc compile + run  ($out)" }
}

# 2) fsi: execute a script and check its result.
$fsx = Join-Path $WorkDir 'smoke.fsx'
@'
let primes n =
    let isPrime x = x > 1 && (Seq.forall (fun d -> x % d <> 0) (seq { 2 .. int (sqrt (float x)) }))
    [2..n] |> List.filter isPrime |> List.length
let count = primes 100
printfn "FSI_OK %d" count
if count <> 25 then exit 1
'@ | Set-Content $fsx -Encoding UTF8

$fsiOut = & $fsi --nologo --exec $fsx
if ($LASTEXITCODE -ne 0 -or ($fsiOut -notmatch 'FSI_OK 25')) { Write-Host "SMOKE FAIL: fsi script wrong result: $fsiOut (exit $LASTEXITCODE)"; $failures++ }
else { Write-Host "  [PASS] fsi script execute  ($fsiOut)" }

Remove-Item $WorkDir -Recurse -Force -ErrorAction SilentlyContinue

if ($failures -gt 0) { Write-Host "Smoke tests FAILED ($failures)"; exit 1 }
Write-Host "All smoke tests passed."
exit 0
