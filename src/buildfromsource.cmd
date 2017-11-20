@if "%_echo%"=="" echo off 

set __scriptpath=%~dp0

rem build tools
dotnet restore %__scriptpath%buildtools\fslex\fslex.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet  publish %__scriptpath%buildtools\fslex\fslex.fsproj -o %__scriptpath%..\Tools\fslex
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet restore %__scriptpath%buildtools\fsyacc\fsyacc.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet  publish %__scriptpath%buildtools\fsyacc\fsyacc.fsproj -o %__scriptpath%..\Tools\fsyacc
if ERRORLEVEL 1 echo Error: failed  && goto :failure

rem build and pack tools
dotnet restore %__scriptpath%buildfromsource\FSharp.Compiler.nuget\FSharp.Compiler.nuget.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet pack %__scriptpath%buildfromsource\FSharp.Compiler.nuget\FSharp.Compiler.nuget.fsproj -c Release
if ERRORLEVEL 1 echo Error: failed  && goto :failure

goto :success

REM ------ exit -------------------------------------
:failure
endlocal
exit /b 1

:success
endlocal
exit /b 0
