@echo off
setlocal enabledelayedexpansion
rem ============================================================================
rem  F# 15.9 CI entry point. Restores from dotnet-public only (agents can't reach
rem  nuget.org), then builds: net9-SDK proto compiler -> product -> vsintegration IDE.
rem ============================================================================

set "_root=%~dp0.."

rem --- Step 1: restore packages.config into .\packages first - the legacy projects import
rem    packages\MicroBuild.Core props at evaluation time, before the Arcade restore. ---
echo ---------------- Restoring packages.config (dotnet-public) ----------------
"%~dp0..\.nuget\NuGet.exe" restore "%~dp0..\packages.config" -PackagesDirectory "%~dp0..\packages" -ConfigFile "%~dp0..\NuGet.config" -NonInteractive -Verbosity quiet
if errorlevel 1 ( echo Error: packages.config restore failed 1>&2 & exit /b 1 )

rem --- Step 2: Arcade restore via eng\sdk-init.proj (a trivial project, since the all-
rem    packages.config FSharp.sln has nothing to restore) to acquire the SDK + toolset. ---
echo ---------------- Arcade restore + SDK acquisition ----------------
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -restore -configuration Release -projects '%~dp0sdk-init.proj' /p:DisableLocalization=true; exit $LASTEXITCODE"
if errorlevel 1 ( echo Error: Arcade restore / SDK acquisition failed 1>&2 & exit /b 1 )

rem --- Step 3: build the proto/bootstrap compiler with the .NET 9 SDK ---
echo ---------------- Building proto (bootstrap) compiler ----------------
powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0build-proto-net9.ps1"
if errorlevel 1 ( echo Error: proto compiler build failed 1>&2 & exit /b 1 )

rem --- Step 4: real product build via Arcade (uses Proto\net40\bin). -m:1 avoids the
rem    legacy net40\bin race; DisableLocalization defers XliffTasks. No signing/pack here. ---
echo ---------------- Building product (real) ----------------
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -build -configuration Release -projects '%_root%\FSharp.Product.sln' /m:1 /p:DisableLocalization=true; exit $LASTEXITCODE"
if errorlevel 1 ( echo Error: product build failed 1>&2 & exit /b 1 )

rem --- Step 5: vsintegration IDE build (FSharp.Insertion.sln). GeneratePkgDefFile=false skips
rem    the VsSDK CreatePkgDef task, which loads the delay-signed assembly and SN-validates it;
rem    pkgdef/VSIX/signing are produced later in the insertion build. ---
echo ---------------- Building vsintegration IDE ----------------
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -restore -build -configuration Release -projects '%_root%\FSharp.Insertion.sln' /m:1 /p:DisableLocalization=true /p:GeneratePkgDefFile=false; exit $LASTEXITCODE"
exit /b %ERRORLEVEL%
