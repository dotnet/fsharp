@echo off

rem
rem End to end tests for TypeProviders
rem To succeed it depends on both the coreclr compiler and the net40 desktop compiler being built
rem It only runs under ci_part3
rem

setlocal
set __scriptpath=%~dp0

rem
rem Build typeprovider package with desktop compiler
rem Test it with both desktop and coreclr compilers
rem

if not '%NUGET_PACKAGES%' == '' rd %NUGET_PACKAGES%\ComboProvider /s /q
taskkill /F /IM dotnet.exe

@echo %__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe pack ComboProvider\ComboProvider\ComboProvider.fsproj -o %~dp0artifacts -c release -v minimal -p:FSharpTestCompilerVersion=net40 -nr=false
%__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe pack ComboProvider\ComboProvider\ComboProvider.fsproj -o %~dp0artifacts -c release -v minimal -p:FSharpTestCompilerVersion=net40 -nr=false
@if ERRORLEVEL 1 echo Error: ComboProvider failed  && goto :failure

@echo %__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe test ComboProvider\ComboProvider.Tests\ComboProvider.Tests.fsproj -c release -v minimal -p:TestTargetFramework=net461 -p:FSharpTestCompilerVersion=net40 -nr=false
%__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe test ComboProvider\ComboProvider.Tests\ComboProvider.Tests.fsproj -c release -v minimal -p:TestTargetFramework=net461 -p:FSharpTestCompilerVersion=net40 -nr=false
@if ERRORLEVEL 1 echo Error: ComboProvider failed  && goto :failure

@echo %__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe test ComboProvider\ComboProvider.Tests\ComboProvider.Tests.fsproj -c release -v minimal -p:TestTargetFramework=netcoreapp3.0 -p:FSharpTestCompilerVersion=coreclr -nr=false
%__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe test ComboProvider\ComboProvider.Tests\ComboProvider.Tests.fsproj -c release -v minimal -p:TestTargetFramework=netcoreapp3.0 -p:FSharpTestCompilerVersion=coreclr -nr=false
@if ERRORLEVEL 1 echo Error: ComboProviderProvider failed  && goto :failure

rem
rem Build typeprovider package with coreclr compiler
rem Test it with both desktop and coreclr compilers
rem
if not '%NUGET_PACKAGES%' == '' rd %NUGET_PACKAGES%\ComboProvider /s /q

@echo %__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe pack ComboProvider\ComboProvider\ComboProvider.fsproj -o %~dp0artifacts -c release -v minimal -p:FSharpTestCompilerVersion=coreclr -nr=false
%__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe pack ComboProvider\ComboProvider\ComboProvider.fsproj -o %~dp0artifacts -c release -v minimal -p:FSharpTestCompilerVersion=coreclr -nr=false
@if ERRORLEVEL 1 echo Error: ComboProviderProvider failed  && goto :failure

@echo%__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe test ComboProvider\ComboProvider.Tests\ComboProvider.Tests.fsproj -c release -v minimal -p:TestTargetFramework=net461 -p:FSharpTestCompilerVersion=net40 -nr=false
%__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe test ComboProvider\ComboProvider.Tests\ComboProvider.Tests.fsproj -c release -v minimal -p:TestTargetFramework=net461 -p:FSharpTestCompilerVersion=net40 -nr=false
@if ERRORLEVEL 1 echo Error: TestBasicProvider failed  && goto :failure

@echo %__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe test ComboProvider\ComboProvider.Tests\ComboProvider.Tests.fsproj -v minimal -p:TestTargetFramework=netcoreapp3.0 -p:FSharpTestCompilerVersion=coreclr -nr=false
%__scriptpath%..\..\..\artifacts\toolset\dotnet\dotnet.exe test ComboProvider\ComboProvider.Tests\ComboProvider.Tests.fsproj -c release -v minimal -p:TestTargetFramework=netcoreapp3.0 -p:FSharpTestCompilerVersion=coreclr -nr=false
@if ERRORLEVEL 1 echo Error: ComboProvider failed  && goto :failure

:success
endlocal
echo Succeeded
exit /b 0

:failure
endlocal
echo Failed
exit /b 1
