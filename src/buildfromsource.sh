#!/bin/sh -e

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)

# build tools
dotnet restore $__scriptpath/buildtools/fssrgen/fssrgen.fsproj
dotnet publish $__scriptpath/buildtools/fssrgen/fssrgen.fsproj -o $__scriptpath/../Tools/fssrgen
dotnet restore $__scriptpath/buildtools/fslex/fslex.fsproj
dotnet publish $__scriptpath/buildtools/fslex/fslex.fsproj -o $__scriptpath/../Tools/fslex
dotnet restore $__scriptpath/buildtools/fsyacc/fsyacc.fsproj
dotnet publish $__scriptpath/buildtools/fsyacc/fsyacc.fsproj -o $__scriptpath/../Tools/fsyacc

# build tools
dotnet restore $__scriptpath/fsharp/FSharp.Build/FSharp.Build.BuildFromSource.fsproj
dotnet publish $__scriptpath/fsharp/FSharp.Build/FSharp.Build.BuildFromSource.fsproj

dotnet restore $__scriptpath/fsharp/fsi/Fsi.BuildFromSource.fsproj
dotnet publish fsharp/fsi/Fsi.BuildFromSource.fsproj

dotnet restore $__scriptpath/fsharp/Fsc/Fsc.BuildFromSource.fsproj
dotnet publish $__scriptpath/fsharp/Fsc/Fsc.BuildFromSource.fsproj

# build and pack tools
dotnet restore $__scriptpath/fsharp/FSharp.Compiler.nuget/FSharp.Compiler.nuget.BuildFromSource.fsproj
dotnet pack $__scriptpath/fsharp/FSharp.Compiler.nuget/FSharp.Compiler.nuget.BuildFromSource.fsproj -c release
