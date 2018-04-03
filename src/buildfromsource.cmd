@if "%_echo%"=="" echo off 

set __scriptpath=%~dp0

rem build tools
dotnet restore %__scriptpath%buildtools\fslex\fslex.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet publish %__scriptpath%buildtools\fslex\fslex.fsproj -o %__scriptpath%..\Tools\fslex /p:BuildFromSource=true
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet restore %__scriptpath%buildtools\fsyacc\fsyacc.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet publish %__scriptpath%buildtools\fsyacc\fsyacc.fsproj -o %__scriptpath%..\Tools\fsyacc /p:BuildFromSource=true
if ERRORLEVEL 1 echo Error: failed  && goto :failure

rem build and pack tools
dotnet restore %__scriptpath%fsharp\FSharp.Compiler.nuget\BuildFromSource\FSharp.Compiler.nuget.fsproj
if ERRORLEVEL 1 echo Error: failed  && goto :failure
dotnet pack %__scriptpath%fsharp\FSharp.Compiler.nuget\BuildFromSource\FSharp.Compiler.nuget.fsproj -c Release /p:BuildFromSource=true
if ERRORLEVEL 1 echo Error: failed  && goto :failure

goto :success

REM ------ exit -------------------------------------
:failure
endlocal
exit /b 1

:success
endlocal
exit /b 0
