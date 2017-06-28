@if "%_echo%"=="" echo off 

set __scriptpath=%~dp0

rem build tools
dotnet restore %__scriptpath%buildtools\fssrgen\fssrgen.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet publish %__scriptpath%buildtools\fssrgen\fssrgen.fsproj -o %__scriptpath%..\Tools\fssrgen
if ERRORLEVEL 1 echo Error: failed  && goto :failure

dotnet restore %__scriptpath%buildtools\fslex\fslex.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet  publish %__scriptpath%buildtools\fslex\fslex.fsproj -o %__scriptpath%..\Tools\fslex
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet restore %__scriptpath%buildtools\fsyacc\fsyacc.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet  publish %__scriptpath%buildtools\fsyacc\fsyacc.fsproj -o %__scriptpath%..\Tools\fsyacc
if ERRORLEVEL 1 echo Error: failed  && goto :failure

rem build and pack tools
dotnet restore %__scriptpath%fsharp\FSharp.Compiler.nuget\FSharp.Compiler.nuget.BuildFromSource.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet pack %__scriptpath%fsharp\FSharp.Compiler.nuget\FSharp.Compiler.nuget.BuildFromSource.fsproj -c release
if ERRORLEVEL 1 echo Error: failed  && goto :failure

goto :success

REM ------ exit -------------------------------------
:failure
endlocal
exit /b 1

:success
endlocal
exit /b 0
