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
rem    (2.10.0-beta2-72429-17) is not on any public/approved feed, so it is vendored as a committed local
rem    NuGet feed (setup\dependencies\RoslynServiced, wired into NuGet.Config). Restore therefore uses only
rem    the committed config feeds - no devdiv feed, no cross-org service connection. Gated on BUILD_INSERTION
rem    so product CI stays green if off. ---
if not defined BUILD_INSERTION ( echo vsintegration/insertion skipped ^(set BUILD_INSERTION=1 to build the signed VSIX^) & exit /b 0 )
rem    Use the Arcade-provisioned .NET SDK (matches global.json), NOT the agent's system dotnet on PATH
rem    (which is older than global.json and makes 'dotnet restore' fail with "A compatible .NET SDK was
rem    not found"). DOTNET_MULTILEVEL_LOOKUP=0 keeps the muxer from falling back to the system install.
set "_dotnet=%_root%\.dotnet\dotnet.exe"
if not exist "%_dotnet%" set "_dotnet=dotnet"
set "DOTNET_ROOT=%_root%\.dotnet"
set "DOTNET_MULTILEVEL_LOOKUP=0"
rem    Restore into Arcade's repo-local .packages folder (the product build's $(NuGetPackageRoot), which is
rem    $(NUGET_PACKAGES) per FSharpBuild.Directory.Build.props). Otherwise the restore lands in the global
rem    packages folder and the Exists()-gated VSSDK.BuildTools import in vsintegration/Directory.Build.targets
rem    (which defines the VSCTCompile target) is skipped -> "target VSCTCompile does not exist". build.ps1
rem    only sets NUGET_PACKAGES when unset, so it inherits this and stays consistent. ---
set "NUGET_PACKAGES=%_root%\.packages\"
rem    The VSIX output group (declared by FSharp.Compiler.Private) ships net40\bin\System.ValueTuple.dll,
rem    but that reference resolves from a netstandard1.0 lib which modern RAR refuses to copy into a net40
rem    output, so the DLL is absent from net40\bin (product/tests never needed it; only VSIX packaging does).
rem    Stage the net40-compatible variant from the restored package so the VSIX can embed it. ---
set "_vtSrc=%_root%\packages\System.ValueTuple.4.3.0\lib\portable-net40+sl4+win8+wp8\System.ValueTuple.dll"
if not exist "%_root%\Release\net40\bin\System.ValueTuple.dll" if exist "%_vtSrc%" ( echo Staging System.ValueTuple.dll into net40\bin & copy /Y "%_vtSrc%" "%_root%\Release\net40\bin\" >nul )
rem    Stage the vendored FSharp.Data.TypeProviders.dll into net40\bin. It is the nuget.org-only legacy type-
rem    providers redist (not on any approved feed, agent is firewalled off nuget.org), so it is vendored in the
rem    repo (setup\dependencies\TypeProviders) and staged here for the insertion Compiler component to package. ---
set "_tpSrc=%_root%\setup\dependencies\TypeProviders\FSharp.Data.TypeProviders.dll"
if not exist "%_root%\Release\net40\bin\FSharp.Data.TypeProviders.dll" if exist "%_tpSrc%" ( echo Staging FSharp.Data.TypeProviders.dll into net40\bin & copy /Y "%_tpSrc%" "%_root%\Release\net40\bin\" >nul )
rem    Restore uses the committed NuGet.Config feeds only (which now include the vendored serviced Roslyn
rem    local feed at setup\dependencies\RoslynServiced). No --source override / devdiv feed is needed. ---
echo ---------------- Restoring + building vsintegration insertion (using %_dotnet%) ----------------
"%_dotnet%" restore "%_root%\FSharp.Insertion.sln" --configfile "%_root%\NuGet.Config"
if errorlevel 1 ( echo Error: insertion restore failed 1>&2 & exit /b 1 )
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -build -configuration Release -projects '%_root%\FSharp.Insertion.sln' /m:1 /p:DisableLocalization=true /p:GeneratePkgDefFile=false; exit $LASTEXITCODE"
if errorlevel 1 ( echo Error: vsintegration build failed 1>&2 & exit /b 1 )
"%_dotnet%" restore "%_root%\vsintegration\Vsix\VisualFSharpFull\VisualFSharpFull.csproj" --configfile "%_root%\NuGet.Config"
if errorlevel 1 ( echo Error: VSIX restore failed 1>&2 & exit /b 1 )
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -build -configuration Release -projects '%_root%\vsintegration\Vsix\VisualFSharpFull\VisualFSharpFull.csproj' /m:1 /p:DisableLocalization=true /p:GeneratePkgDefFile=false; exit $LASTEXITCODE"
if errorlevel 1 ( echo Error: VSIX build failed 1>&2 & exit /b 1 )

rem --- Step 5b (opt-in via RUN_VSINTEGRATION_TESTS): VS IDE unit tests (VisualFSharp.UnitTests). Gated OFF by
rem    default: the test build needs DESKTOP msbuild (dotnet/Core msbuild can't build the VSSDK+WPF projects -
rem    System.IO.Packaging / XAML InitializeComponent), and the headless STA/MEF run is still unvalidated. The
rem    repoints + version props + run-vsintegration-tests.ps1 are in place for when this is picked back up. ---
if defined RUN_VSINTEGRATION_TESTS (
  echo ---------------- Running VS IDE unit tests ^(opt-in^) ----------------
  powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0run-vsintegration-tests.ps1" -Configuration Release -ResultsDir "%_root%\artifacts\VsixTestResults"
  if errorlevel 1 ( echo WARNING: VS IDE unit tests reported failure ^(non-fatal^) 1>&2 )
)

rem --- Step 6 (with Step 5): sign the insertion VSIX and the F# assemblies inside it via the Arcade SignTool,
rem    driven by eng\Signing.props. The 1ES template's mb.signing block installs the MicroBuild signing plugin;
rem    SignType=Real does production signing (needs the signing service authorized for the pipeline),
rem    SignType=Test uses test certs (proves the mechanism, no authorization needed). Skipped when SignType is
rem    unset, so product-only CI is unaffected. ---
if not defined SignType ( echo signing skipped ^(set SignType=Test^|Real to sign the VSIX^) & exit /b 0 )
echo ---------------- Signing insertion VSIX ^(SignType=%SignType%^) ----------------
rem    -warnAsError:$false: the Arcade SignTool emits SIGN001 (a warning) for the F# IDE assemblies
rem    (FSharp.Editor/LanguageService/ProjectSystem/...) because their assembly copyright attribute is empty,
rem    so its heuristic can't confirm they are Microsoft-owned; it still signs them with the Microsoft cert,
rem    exactly as the original 15.9 AssemblySignToolData.json intended. -ci would otherwise promote that
rem    warning to an error. This pass does no compilation, so nothing else needs warn-as-error.
rem    OfficialBuildId (from the pipeline's Build.BuildNumber) makes Arcade apply real signatures instead of
rem    dry-running, matching dotnet/fsharp main's official signed build.
set "_officialArg="
if defined OfficialBuildId set "_officialArg=/p:OfficialBuildId=%OfficialBuildId%"
powershell -NoProfile -ExecutionPolicy ByPass -Command "& '%~dp0common\build.ps1' -ci -sign -warnAsError:$false -configuration Release /p:DotNetSignType=%SignType% %_officialArg% /p:MicroBuild_SigningEnabled=true /p:TeamName=FSharp; exit $LASTEXITCODE"
if errorlevel 1 ( echo Error: signing failed 1>&2 & exit /b 1 )

rem --- Step 7 (opt-in via BUILD_VSMAN): lean VS insertion .vsman drop. Produces a Visual Studio setup manifest
rem    (Microsoft.FSharp.vsman + payload) describing ONLY the signed VisualFSharpFull.vsix (compiler + IDE -
rem    where the sqlite CVE lives), via the MODERN MicroBuild SWIX plugin (Microsoft.VisualStudioEng.MicroBuild.
rem    Plugins.SwixBuild) restored like main from the devdiv dotnet-core-internal-tooling feed (authenticated by
rem    the pipeline's NuGetAuthenticate). Uses DESKTOP MSBuild (the .vsmanproj + SWIX tasks are .NET Framework).
rem    Gated so a drop-authoring failure never affects the proven product/tests/signed-VSIX path. ---
if not defined BUILD_VSMAN ( echo vsman drop skipped ^(set BUILD_VSMAN=1 to build the lean .vsman insertion drop^) & exit /b 0 )
echo ---------------- Restoring modern MicroBuild SwixBuild plugin (internal-tools feed) ----------------
set "_dotnet=%_root%\.dotnet\dotnet.exe"
if not exist "%_dotnet%" set "_dotnet=dotnet"
set "DOTNET_ROOT=%_root%\.dotnet"
set "DOTNET_MULTILEVEL_LOOKUP=0"
set "NUGET_PACKAGES=%_root%\.packages\"
rem    Restore sources = the committed public feeds PLUS the devdiv dotnet-core-internal-tooling feed (which
rem    carries the SwixBuild plugin), passed as --source so nothing devdiv is written into a committed config
rem    (CFS stays green). NuGetAuthenticate (pipeline step) supplied the credential for the devdiv feed.
set "_itFeed=https://pkgs.dev.azure.com/devdiv/_packaging/dotnet-core-internal-tooling/nuget/v3/index.json"
set "_srcArgs=--source "%_itFeed%""
for /f "usebackq delims=" %%U in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "([xml](Get-Content -Raw '%_root%\NuGet.Config')).configuration.packageSources.add | Where-Object { $_.value -like 'http*' } | ForEach-Object { $_.value }"`) do set "_srcArgs=!_srcArgs! --source "%%U""
"%_dotnet%" restore "%_root%\eng\restore-swixplugin.proj" --configfile "%_root%\NuGet.Config" !_srcArgs!
if errorlevel 1 ( echo Error: SwixBuild plugin restore failed 1>&2 & exit /b 1 )
set "SwixPluginDir="
echo Locating the restored SwixBuild plugin build folder under "%NUGET_PACKAGES%"...
for /f "usebackq delims=" %%D in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$p = Get-ChildItem -Path '%NUGET_PACKAGES%' -Recurse -Filter 'Microsoft.VisualStudio.Setup.Tools.targets' -ErrorAction SilentlyContinue ^| Where-Object { $_.FullName -match 'swixbuild' } ^| Select-Object -First 1; if ($p) { $p.DirectoryName }"`) do set "SwixPluginDir=%%D"
if not defined SwixPluginDir (
  echo Error: modern SwixBuild plugin build folder not found after restore 1>&2
  dir /s /b "%NUGET_PACKAGES%microsoft.visualstudioeng.microbuild.plugins.swixbuild" 2>nul
  exit /b 1
)
echo SwixPluginDir=%SwixPluginDir%
echo ---------------- Building lean VS insertion .vsman drop ----------------
set "_vswhere=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
set "_msbuild="
for /f "usebackq tokens=*" %%M in (`"%_vswhere%" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "_msbuild=%%M"
if not defined _msbuild ( echo Error: desktop MSBuild not found via vswhere 1>&2 & exit /b 1 )
echo Using desktop MSBuild: %_msbuild%
"%_msbuild%" "%_root%\setup\Swix\Microsoft.FSharp.Lean.vsmanproj" /p:Configuration=Release /p:SwixPluginDir="%SwixPluginDir%" /p:ManifestBuildVersion=15.9.%OfficialBuildId% /bl:"%_root%\artifacts\log\Release\vsman.binlog"
if errorlevel 1 ( echo Error: .vsman drop build failed 1>&2 & exit /b 1 )
echo ---------------- Lean .vsman insertion drop built ----------------
if exist "%_root%\Release\insertion" ( dir "%_root%\Release\insertion" ) else ( echo Error: Release\insertion not produced 1>&2 & exit /b 1 )
exit /b 0
