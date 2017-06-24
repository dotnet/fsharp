@echo off
rem build tools
dotnet restore buildtools\fssrgen\fssrgen.fsproj
dotnet publish buildtools\fssrgen\fssrgen.fsproj -o %~dp0..\Tools\fssrgen
dotnet restore buildtools\fslex\fslex.fsproj
dotnet publish buildtools\fslex\fslex.fsproj -o %~dp0..\Tools\fslex
dotnet restore buildtools\fsyacc\fsyacc.fsproj
dotnet publish buildtools\fsyacc\fsyacc.fsproj -o %~dp0..\Tools\fsyacc

rem build tools
dotnet restore fsharp\FSharp.Build\FSharp.Build.BuildFromSource.fsproj
dotnet publish fsharp\FSharp.Build\FSharp.Build.BuildFromSource.fsproj


