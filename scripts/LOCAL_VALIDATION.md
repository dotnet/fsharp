# scripts/LOCAL_VALIDATION.md

Local pre-push validation for `release/dev15.9.x`. Per Constraint 9 in `plan.md` (session workspace).

## Why

Every contributor runs the validation gates locally before pushing to `release/dev15.9.x`. Catches restore failures, broken build, broken bootstrap, broken FSI/FSC behaviour, and most regressions BEFORE they hit CI. CI repeats the same gates server-side; mismatch between local-green and CI-red is the single highest-value smell to investigate.

## Prerequisites (one-time)

- Windows 10/11 with VS 2017 15.9 installed (provides MSBuild 15, .NET Framework reference assemblies for net20/40/45, VS SDK 15.0). Verify: `vswhere -version "[15.0,16.0)" -property installationPath` returns a path.
- Git, PowerShell 5.1+ (or pwsh 7+).
- Network access to `api.nuget.org` and dnceng/azure-public NuGet feeds.

## Quick run (G1+G2+G3)

```pwsh
pwsh -NoProfile -File scripts\local-validate.ps1
# or
powershell -NoProfile -File scripts\local-validate.ps1 -Configuration Debug
```

Expected wall-time: 30–45 min on a clean clone (init-tools.cmd downloads ~1 GB legacy SDKs first run), 15–20 min on subsequent runs.

## Gates explained

### G1 — Build-script smoke (compiler-only)

```cmd
set FSC_BUILD_SETUP=0
build.cmd net40 Debug
```

Exit criteria:
- `init-tools.cmd` succeeds; `Tools\<BuildToolsVersion>\init-tools.completed0` exists.
- `build.cmd net40 Debug` exits 0.
- `Debug\net40\bin\fsc.exe` exists and `fsc.exe --help` prints usage.

`FSC_BUILD_SETUP=0` skips Swix/setup to keep G1 fast. G1 does NOT exercise signing, swix, or insertion (those run in G4 + L5).

### G2 — Bootstrap (proto-fsc → real-fsc → FSharp.Core)

```cmd
MSBuild.exe src\fsharp-proto-build.proj /t:Build /p:Configuration=Proto;BUILD_PROTO_WITH_CORECLR_LKG=1;DisableLocalization=true /bl:proto.binlog
MSBuild.exe build-everything.proj      /t:Build /p:Configuration=Release;BUILD_NET40_FSHARP_CORE=1;BUILD_NET40=1 /bl:real.binlog
```

Exit criteria:
- `Proto\net40\bin\fsc.exe` exists.
- `Release\net40\bin\fsc.exe` exists.
- `Release\net40\bin\FSharp.Core.dll` exists.
- `[System.Reflection.AssemblyName]::GetAssemblyName(...).Version` reports `4.5.0.0` (matches `FSCoreVersion` in `build/targets/AssemblyVersions.props`).

### G3 — Functional smoke (FSI sanity, FSC produces runnable PE, records/DUs/CEs, #r, .fsi consumption)

Six sub-tests; all must pass. See `scripts\local-validate.ps1` step "G3" for the exact commands.

## L4 (full local CI emulation)

Not part of `local-validate.ps1` — run separately when you've changed pipeline/sign/insertion bits:

```cmd
set PB_SIGNTYPE=test
set PB_SKIPTESTS=true
build.cmd microbuild release

REM Then:
powershell -NoProfile -File scripts\verify-vsix-targets.ps1 -InsertionDir Release\insertion
powershell -NoProfile -File scripts\compare-insertion-vs-rtm.ps1 -Built Release\insertion -Rtm <local-cache-of-RTM>
```

L4 exit criteria are enforced by the two scripts plus a hard-fail nupkg-leak check (Constraint 4):

```pwsh
$leaks = Get-ChildItem Release -Recurse -Filter '*.nupkg' -ErrorAction SilentlyContinue
if ($leaks) { $leaks; throw 'nupkg leaked! Constraint 4 violated' }
```

Wall-time: 70–90 min for L4 (signing + 14 locales × multiple swixprojs).

## L4b (VS extension load smoke)

Run in a clean VS 2017 VM. See `scripts\VS15.9_PATCH_SMOKE.md`.

## On failure

- **G1 restore failure** → check `Q:\source\fsharp\refs\fsharp-15.9-package-availability.csv`; the failing package may need a feed mirror or a different feed source. See `INTERNAL.md` "Tickets / external dependencies".
- **G1 compile failure** → check `Debug\fsharp_build_log.log` (set `BUILD_DIAG=/v:detailed` for verbose).
- **G2 proto failure** → most likely `packages\FSharp.Compiler.Tools.4.1.27\tools\fsc.exe` did not restore (netfx fallback) OR `Tools\dotnet20\sdk\<DotnetCLIToolsVersion>\FSharp\fsc.exe` is missing (CoreCLR LKG path). Check `init-tools.log`.
- **G2 wrong AssemblyVersion** → check `build/targets/AssemblyVersions.props`.
- **G3 FSI crash** → suspect FSharp.Core bound to wrong CLR runtime. Diff `Release\net40\bin\fsi.exe.config` vs RTM.
- **G3 #r fails** → `fsi.exe.config` BindingRedirects are wrong.
- **L4 nupkg leak** → B10 patch reverted in `build.cmd` line 214 (or other modes accidentally enabled). Re-apply patch.

## Iteration discipline (G9)

When a gate fails, fix it locally and re-run **the same gate**. Do NOT push to CI to "see if CI sees it differently". Most regressions surface earlier and cheaper at L4 than L5.
