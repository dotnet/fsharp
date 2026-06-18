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

rem --- Signing defaults (Block 9m) ---
rem    SignType=Test validates the Arcade SignTool manifest + file discovery with no
rem    operator/prod-signing-service dependency (in-build test certs). SignType=Real
rem    (set by the pipeline once it is authorized for prod signing + the MicroBuild
rem    signing plugin is enabled via templateContext.mb.signing) does the real sign.
if "%SignType%"=="" set "SignType=Test"
if "%TeamName%"=="" set "TeamName=FSharp"

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

rem --- Step 4: real build via the Arcade engine (uses Proto\net40\bin) ---
rem    -m:1 forces single-proc msbuild (the legacy build shares a Release\net40\bin dir
rem    that races under multi-proc). DisableLocalization=true defers XliffTasks.
rem    FSharp.Product.sln is FSharp.sln minus the unit-test projects: the VS insertion needs
rem    only the product, and the test projects pull build-time fsi (subst.fsx) + test packages
rem    that are out of scope for the product build. Tests are wired up in a later step.
rem
rem    SIGNING is intentionally NOT done here. Arcade signs *containers* (the nupkgs from
rem    -pack, and the VSIX) and recursively signs their contents; a product-only build with
rem    no -pack (we must NOT produce an FSharp.Core nupkg) and no VSIX yet has an empty
rem    ItemsToSign list ("List of files to sign is empty", Sign.proj). Signing is therefore
rem    sequenced into the insertion build (EPIC I), where the VSIX is the sign container and
rem    Arcade signs the embedded FSharp.Core.dll / compiler / e_sqlite3.dll with no nupkg.
rem    %SignType% / %TeamName% defaults above are ready scaffolding for that step.
echo ---------------- Building product (real) ----------------
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -build -configuration Release -projects '%_root%\FSharp.Product.sln' /m:1 /p:DisableLocalization=true; exit $LASTEXITCODE"
if errorlevel 1 ( echo Error: product build failed 1>&2 & exit /b 1 )

rem --- Step 5: vsintegration IDE build (EPIC V, NON-FATAL while iterating) ---
rem    Builds the VS IDE assemblies (vsintegration\src) via FSharp.Insertion.sln so the
rem    signed VS insertion (EPIC I) can later package the VSIX. The product projects are a
rem    superset prerequisite (already built in Step 4 -> incremental here). This step is
rem    deliberately NON-FATAL: EPIC V is mid-iteration (Block 9n repointed the MSBuild deps;
rem    BC30389/VSIX work remains) and a vsintegration failure must NOT regress the proven-
rem    green product build at 213261902. Read THIS step's log for the PASS/FAIL + real error;
rem    it is made fatal (and folded into the signed insertion build) once green.
echo ---------------- Building vsintegration IDE (EPIC V, non-fatal) ----------------
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -restore -build -configuration Release -projects '%_root%\FSharp.Insertion.sln' /m:1 /p:DisableLocalization=true; exit $LASTEXITCODE"
if errorlevel 1 ( echo ##vso[task.logissue type=warning]EPIC V: vsintegration IDE build FAILED ^(non-fatal^) - see log above ) else ( echo EPIC V: vsintegration IDE build PASSED )
exit /b 0
