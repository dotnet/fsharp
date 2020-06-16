@echo off

rem
rem End to end tests for TypeProviders
rem To succeed it depends on both the coreclr compiler and the net40 desktop compiler being built
rem It only runs under ci_part3
rem

setlocal
set __scriptpath=%~dp0
set configuration=Debug

:parseargs
if "%1" == "" goto argsdone
if /i "%1" == "-c" goto set_configuration

echo Unsupported argument: %1
goto failure

:set_configuration
set configuration=%2
shift
shift
goto parseargs

:argsdone

pushd %__scriptpath%
rem
rem Build typeprovider package with desktop compiler
rem Test it with both desktop and coreclr compilers
rem

if not '%NUGET_PACKAGES%' == '' rd %NUGET_PACKAGES%\ComboProvider /s /q
taskkill /F /IM dotnet.exe

echo dotnet pack ComboProvider\ComboProvider.fsproj -o %~dp0artifacts -c %configuration% -v minimal -p:FSharpTestCompilerVersion=net40
     dotnet pack ComboProvider\ComboProvider.fsproj -o %~dp0artifacts -c %configuration% -v minimal -p:FSharpTestCompilerVersion=net40
if ERRORLEVEL 1 echo Error: TestComboProvider failed  && goto :failure

echo dotnet test ComboProvider.Tests\ComboProvider.Tests.fsproj -c %configuration% -v minimal -p:TestTargetFramework=net461 -p:FSharpTestCompilerVersion=net40
     dotnet test ComboProvider.Tests\ComboProvider.Tests.fsproj -c %configuration% -v minimal -p:TestTargetFramework=net461 -p:FSharpTestCompilerVersion=net40
if ERRORLEVEL 1 echo Error: TestComboProvider failed  && goto :failure

echo dotnet test ComboProvider.Tests\ComboProvider.Tests.fsproj -c %configuration% -v minimal -p:TestTargetFramework=netcoreapp3.1 -p:FSharpTestCompilerVersion=coreclr
     dotnet test ComboProvider.Tests\ComboProvider.Tests.fsproj -c %configuration% -v minimal -p:TestTargetFramework=netcoreapp3.1 -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 echo Error: TestComboProvider failed  && goto :failure

rem
rem Build typeprovider package with coreclr compiler
rem Test it with both desktop and coreclr compilers
rem
if not '%NUGET_PACKAGES%' == '' rd %NUGET_PACKAGES%\ComboProvider /s /q

echo dotnet pack ComboProvider\ComboProvider.fsproj -o %~dp0artifacts -c %configuration% -v minimal -p:FSharpTestCompilerVersion=coreclr
     dotnet pack ComboProvider\ComboProvider.fsproj -o %~dp0artifacts -c %configuration% -v minimal -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 echo Error: TestComboProvider failed  && goto :failure

echo dotnet test ComboProvider.Tests\ComboProvider.Tests.fsproj -c %configuration% -v minimal -p:TestTargetFramework=net461 -p:FSharpTestCompilerVersion=net40
     dotnet test ComboProvider.Tests\ComboProvider.Tests.fsproj -c %configuration% -v minimal -p:TestTargetFramework=net461 -p:FSharpTestCompilerVersion=net40
if ERRORLEVEL 1 echo Error: TestComboProvider failed  && goto :failure

echo dotnet test ComboProvider.Tests\ComboProvider.Tests.fsproj -v %configuration% -p:TestTargetFramework=netcoreapp3.1 -p:FSharpTestCompilerVersion=coreclr
     dotnet test ComboProvider.Tests\ComboProvider.Tests.fsproj -c %configuration% -v minimal -p:TestTargetFramework=netcoreapp3.1 -p:FSharpTestCompilerVersion=coreclr
if ERRORLEVEL 1 echo Error: TestComboProvider failed  && goto :failure

:success
endlocal
echo Succeeded
popd
exit /b 0

:failure
endlocal
echo Failed
popd
exit /b 1
