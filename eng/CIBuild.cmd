@echo off
setlocal enabledelayedexpansion
rem ============================================================================
rem  F# 15.9 servicing CI build entry point (Block 9i: .NET 9 SDK proto, no nuget.org).
rem
rem  The 15.9 build is 2-phase (LKG proto compiler -> real compiler). We build the
rem  proto with the .NET 9 SDK's bundled F# toolset (see eng\build-proto-net9.ps1),
rem  which removes every nuget.org-only seed (FSharp.Compiler.Tools 4.1.27,
rem  Microsoft.VisualFSharp.Msbuild.15.0, FsLexYacc 7.0.6). All remaining build
rem  dependencies are restored from dotnet-public (CFS-compliant; the nuget.org
rem  upstream is fetched server-side, which works on the network-isolated agents).
rem ============================================================================

set "_root=%~dp0.."

rem --- Step 1: install the .NET SDK (global.json tools.dotnet) + Arcade restore ---
rem    DisableLocalization=true so the project evaluation skips the XliffTasks import
rem    (its packages.config-style path is not on the Arcade restore graph).
echo ---------------- Arcade restore + SDK acquisition ----------------
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -restore -configuration Release -projects '%_root%\FSharp.sln' /p:DisableLocalization=true; exit $LASTEXITCODE"
if errorlevel 1 ( echo Error: Arcade restore / SDK acquisition failed 1>&2 & exit /b 1 )

rem --- Step 2: restore the legacy packages.config HintPath deps from dotnet-public ---
rem    No explicit -Source: NuGet.config routes to the approved dotnet-public feed
rem    (whose upstream is fetched server-side), so the agent never contacts the public
rem    NuGet gallery directly. This is CFS-compliant and works on isolated agents.
echo ---------------- Restoring packages.config (dotnet-public) ----------------
"%~dp0..\.nuget\NuGet.exe" restore "%~dp0..\packages.config" -PackagesDirectory "%~dp0..\packages" -ConfigFile "%~dp0..\NuGet.config" -NonInteractive -Verbosity quiet
if errorlevel 1 ( echo Error: packages.config restore failed 1>&2 & exit /b 1 )

rem --- Step 3: build the proto/bootstrap compiler with the .NET 9 SDK ---
echo ---------------- Building proto (bootstrap) compiler ----------------
powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0build-proto-net9.ps1"
if errorlevel 1 ( echo Error: proto compiler build failed 1>&2 & exit /b 1 )

rem --- Step 4: real build via the Arcade engine (uses Proto\net40\bin) ---
rem    -m:1 forces single-proc msbuild: the legacy build copies every net40 project's
rem    output into a shared Release\net40\bin dir, which races under multi-proc.
rem    DisableLocalization=true defers XliffTasks (loc satellites handled separately).
echo ---------------- Building product (real) ----------------
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -build -configuration Release -projects '%_root%\FSharp.sln' /m:1 /p:DisableLocalization=true %*; exit $LASTEXITCODE"
exit /b %ERRORLEVEL%
