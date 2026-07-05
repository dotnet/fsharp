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

rem --- Step 4a: register strong-name verification skip for the F# public key so the delay-signed
rem    product binaries (fsc/fsi/FSharp.Core) can run on the clean agent for testing. The real signed
rem    build applies genuine signatures later; this mirrors the original 15.9 src\update.cmd. ---
echo ---------------- Registering strong-name verification skip ----------------
powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0register-sn-skip.ps1"

rem --- Step 4b: smoke tests. Run the compiler that was just built (fsc/fsi) against real F# code so a
rem    green pipeline proves the produced compiler actually compiles and executes F#, not just that the
rem    build completed. Fast; runs on the build agent (no Helix). Heavier unit/suite tests layer on later. ---
echo ---------------- Running smoke tests ----------------
powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0run-smoke-tests.ps1"
if errorlevel 1 ( echo Error: smoke tests failed 1>&2 & exit /b 1 )

rem --- Step 4c: unit tests. Build + run the 15.9 NUnit unit-test suites (FSharp.Core.UnitTests,
rem    FSharp.Compiler.UnitTests) against the product just built, emitting NUnit XML to
rem    artifacts\TestResults for PublishTestResults. Fails the pipeline on any test failure. ---
echo ---------------- Running unit tests ----------------
powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0run-tests.ps1" -Configuration Release
if errorlevel 1 ( echo Error: unit tests failed 1>&2 & exit /b 1 )

rem --- Step 5 (opt-in): vsintegration IDE + VS insertion VSIX. The serviced VS-2017 editor Roslyn
rem    (2.10.0-beta2-72429-17) is devdiv-only, so it is restored from the devdiv VS-CoreXtFeeds passed as a
rem    build-time --source (authenticated by the 'DevDiv - VS package feed' service connection via
rem    NuGetAuthenticate). The committed NuGet.Config stays clean (approved feeds only); devdiv is never
rem    written into a repo config. Gated on BUILD_INSERTION so product CI stays green if this is turned off. ---
if not defined BUILD_INSERTION ( echo vsintegration/insertion skipped ^(set BUILD_INSERTION=1 to build the signed VSIX^) & exit /b 0 )
set "_devdivFeed=https://pkgs.dev.azure.com/devdiv/_packaging/VS-CoreXtFeeds/nuget/v3/index.json"
echo ---------------- Restoring + building vsintegration insertion ----------------
dotnet restore "%_root%\FSharp.Insertion.sln" --configfile "%_root%\NuGet.Config" --source "%_devdivFeed%"
if errorlevel 1 ( echo Error: insertion restore failed 1>&2 & exit /b 1 )
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -build -configuration Release -projects '%_root%\FSharp.Insertion.sln' /m:1 /p:DisableLocalization=true /p:GeneratePkgDefFile=false; exit $LASTEXITCODE"
if errorlevel 1 ( echo Error: vsintegration build failed 1>&2 & exit /b 1 )
dotnet restore "%_root%\vsintegration\Vsix\VisualFSharpFull\VisualFSharpFull.csproj" --configfile "%_root%\NuGet.Config" --source "%_devdivFeed%"
if errorlevel 1 ( echo Error: VSIX restore failed 1>&2 & exit /b 1 )
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -build -configuration Release -projects '%_root%\vsintegration\Vsix\VisualFSharpFull\VisualFSharpFull.csproj' /m:1 /p:DisableLocalization=true /p:GeneratePkgDefFile=false; exit $LASTEXITCODE"
exit /b %ERRORLEVEL%
