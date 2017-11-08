#!/bin/sh -e

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)

# build tools
dotnet restore $__scriptpath/buildtools/fslex/fslex.fsproj
dotnet publish $__scriptpath/buildtools/fslex/fslex.fsproj -o $__scriptpath/../Tools/fslex
dotnet restore $__scriptpath/buildtools/fsyacc/fsyacc.fsproj
dotnet publish $__scriptpath/buildtools/fsyacc/fsyacc.fsproj -o $__scriptpath/../Tools/fsyacc

# build tools
dotnet restore $__scriptpath/buildfromsource/FSharp.Build/FSharp.Build.fsproj
dotnet publish $__scriptpath/buildfromsource/FSharp.Build/FSharp.Build.fsproj

dotnet restore $__scriptpath/buildfromsource/Fsi/Fsi.fsproj
dotnet publish $__scriptpath/buildfromsource/Fsi/Fsi.fsproj

dotnet restore $__scriptpath/buildfromsource/Fsc/Fsc.fsproj
dotnet publish $__scriptpath/buildfromsource/Fsc/Fsc.fsproj

# build and pack tools
dotnet restore $__scriptpath/buildfromsource/FSharp.Compiler.nuget/FSharp.Compiler.nuget.fsproj
dotnet pack $__scriptpath/buildfromsource/FSharp.Compiler.nuget/FSharp.Compiler.nuget.fsproj -c Release
