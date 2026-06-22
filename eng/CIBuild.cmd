@echo off
setlocal enabledelayedexpansion
rem ============================================================================
rem  F# 15.9 servicing CI build entry point (Block 9j: .NET 9 SDK proto, no nuget.org).
rem
rem  The 15.9 build is 2-phase (LKG proto compiler -> real compiler). We build the
rem  proto with the .NET 9 SDK's bundled F# toolset (see eng\build-proto-net9.ps1),
rem  needing none of the nuget.org-only seeds. Every remaining dependency resolves
rem  directly from dotnet-public (the network-isolated agents cannot reach nuget.org,
rem  and the dotnet-public upstream is NOT fetched on demand there, so packages.config
rem  must list only packages that are published directly to dotnet-public).
rem ============================================================================

set "_root=%~dp0.."

rem --- Step 1: restore packages.config (HintPath deps) into .\packages FIRST ---
rem    NuGet.exe is standalone (no SDK needed). This must precede the Arcade restore:
rem    the legacy projects import packages\MicroBuild.Core\...\MicroBuild.Core.props at
rem    evaluation time, so .\packages has to be populated before MSBuild loads them.
echo ---------------- Restoring packages.config (dotnet-public) ----------------
"%~dp0..\.nuget\NuGet.exe" restore "%~dp0..\packages.config" -PackagesDirectory "%~dp0..\packages" -ConfigFile "%~dp0..\NuGet.config" -NonInteractive -Verbosity quiet
if errorlevel 1 ( echo Error: packages.config restore failed 1>&2 & exit /b 1 )

rem --- Step 2: install the .NET SDK (global.json tools.dotnet) + Arcade toolset ---
rem    We point -projects at eng\sdk-init.proj (a trivial SDK-style project) rather than
rem    FSharp.sln: the all-packages.config solution has nothing for the SDK to restore and
rem    surfaces a "no project to restore" error. sdk-init.proj restores cleanly while still
rem    making build.ps1 acquire the SDK + Arcade toolset that Step 3/Step 4 need.
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
