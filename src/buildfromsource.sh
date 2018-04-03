#!/bin/sh -e

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)

# build tools
dotnet restore $__scriptpath/buildtools/fslex/fslex.fsproj
dotnet publish $__scriptpath/buildtools/fslex/fslex.fsproj -o $__scriptpath/../Tools/fslex
dotnet restore $__scriptpath/buildtools/fsyacc/fsyacc.fsproj
dotnet publish $__scriptpath/buildtools/fsyacc/fsyacc.fsproj -o $__scriptpath/../Tools/fsyacc

# build tools
dotnet restore $__scriptpath/fsharp/FSharp.Build/FSharp.Build.fsproj
dotnet publish $__scriptpath/fsharp/FSharp.Build/FSharp.Build.fsproj /p:BuildFromSource=true

dotnet restore $__scriptpath/fsharp/fsi/fsi.fsproj
dotnet publish $__scriptpath/fsharp/fsi/fsi.fsproj /p:BuildFromSource=true

dotnet restore $__scriptpath/fsharp/Fsc/Fsc.fsproj
dotnet publish $__scriptpath/fsharp/Fsc/Fsc.fsproj /p:BuildFromSource=true

# build and pack tools
dotnet restore $__scriptpath/fsharp/FSharp.Compiler.nuget/Microsoft.FSharp.Compiler.nuget.proj
dotnet pack $__scriptpath/fsharp/FSharp.Compiler.nuget/Microsoft.FSharp.Compiler.nuget.proj -c Release /p:BuildFromSource=true
