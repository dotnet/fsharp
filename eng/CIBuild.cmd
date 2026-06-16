@echo off
setlocal enabledelayedexpansion
rem F# 15.9 servicing CI build entry point.
rem
rem Block 9b: builds the legacy proto/bootstrap compiler FIRST (the 15.9 self-hosting
rem build is 2-phase: LKG proto compiler -> real compiler), then runs the Arcade-engine
rem real build. The proto build is the part the modern Arcade engine does NOT do.
rem
rem Proto recipe (all verified on the dev box; see RestorePlan Block 9b notes):
rem   /p:Configuration=Proto                 - the proto flavour
rem   /p:VisualStudioVersion=15.0            - FSharp.Build-proto Microsoft.Build.* refs pin
rem                                            Version=$(VisualStudioVersion).0.0 against the 15.0
rem                                            assemblies in Microsoft.VisualFSharp.Msbuild.15.0
rem   /p:SystemCollectionsImmutableVersion=1.5.0 - belt-and-suspenders pin (also in eng/Versions.props);
rem                                            the legacy HintPath needs the netstandard1.0 lib that
rem                                            only 1.5.0 carries
rem   /p:DisableLocalization=true           - XliffTasks deferred
rem   facade injection + v4.7.2 FrameworkPathOverride are baked into src/FSharpSource.targets
rem   (InjectProtoFacades target + _Net45RefPackFallback) so they apply automatically.

set DisableLocalization=true

rem --- locate full VS MSBuild (the legacy proto proj needs full-framework MSBuild) ---
set "_vswhere=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
set "_msbuild="
if exist "%_vswhere%" (
  for /f "usebackq delims=" %%i in (`"%_vswhere%" -latest -prerelease -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "_msbuild=%%i"
)
if not defined _msbuild (
  echo Error: could not locate full VS MSBuild.exe via vswhere. 1>&2
  exit /b 1
)
echo Using MSBuild: !_msbuild!

rem --- Phase 0: restore the legacy packages.config seeds into .\packages ---
rem The proto build's LKG seeds (FSharp.Compiler.Tools 4.1.27 -> Microsoft.FSharp.Targets,
rem Microsoft.VisualFSharp.Msbuild.15.0, System.Collections.Immutable 1.5.0, System.ValueTuple)
rem are declared in packages.config and HintPath'd from .\packages. Arcade's restore populates
rem the NuGet global cache, not .\packages, so on a clean CI agent .\packages is empty and the
rem proto's <Import ...\packages\FSharp.Compiler.Tools.4.1.27\tools\Microsoft.FSharp.Targets>
rem fails MSB4019. Restore packages.config explicitly into .\packages first. All these packages
rem resolve through dotnet-public (the CFS-approved nuget.org mirror) per NuGet.Config.
echo ---------------- Restoring legacy packages.config seeds ----------------
"%~dp0..\.nuget\NuGet.exe" restore "%~dp0..\packages.config" -PackagesDirectory "%~dp0..\packages" -ConfigFile "%~dp0..\NuGet.Config" -NonInteractive -Verbosity quiet
if errorlevel 1 (
  echo Error: legacy packages.config restore failed 1>&2
  exit /b 1
)

rem --- Phase 1: build the proto/bootstrap compiler ---
echo ---------------- Building proto (bootstrap) compiler ----------------
"!_msbuild!" "%~dp0..\src\fsharp-proto-build.proj" ^
  /p:Configuration=Proto ^
  /p:VisualStudioVersion=15.0 ^
  /p:SystemCollectionsImmutableVersion=1.5.0 ^
  /p:DisableLocalization=true ^
  /nologo /v:minimal ^
  /bl:"%~dp0..\artifacts\log\Release\proto.binlog"
if errorlevel 1 (
  echo Error: proto compiler build failed 1>&2
  exit /b 1
)

rem --- Phase 2: real build via the Arcade engine (uses Proto\net40\bin) ---
rem -m:1 forces single-proc msbuild. The legacy 15.9 build copies every net40 project's
rem output into a SHARED Release\net40\{bin,obj} dir via HACK_CopyOutputsToTheProperLocation,
rem and builds the same assemblies (e.g. FSharp.Core.dll) for multiple TFM passes; under
rem multi-proc these collide (MSB3026 copy locks, FS2014 double-write). Every project builds
rem clean in isolation -- the only failures are shared-output races -- so serializing is the
rem correct, deterministic fix. The 15.9 build was historically single-proc.
echo ---------------- Building product (real) ----------------
powershell -NoProfile -ExecutionPolicy ByPass -File "%~dp0common\build.ps1" -ci -restore -build /m:1 %*
exit /b %ERRORLEVEL%