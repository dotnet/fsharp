

# The FSharp.Compiler.Service components and NuGet package

This directory contains the build, packaging, test and documentation-generation logic for the NuGet package ``FSharp.Compiler.Service``.  The source for this NuGet
package is in ``..\src``.

Basically we are packaging up the compiler as a DLL and publishing it as a NuGet package.

## FSharp.Compiler.Service v. FSharp.Compiler.Private

There are subtle differences between FSharp.Compiler.Service and FSharp.Compiler.Private (shipped with the Visual F# Tools)

- FCS has a public API 

- FCS is built against **.NET 4.6.1** and **FSharp.Core NuGet 4.6.2** to give broader reach

- FCS has a NuGet package

- FCS has a .NET Standard 2.0 version in the nuget package

- FCS testing also tests the "Project Cracker" (see below)

- FCS doesn't add the System.ValueTuple.dll reference by default, see ``#if COMPILER_SERVICE_AS_DLL`` in compiler codebase

## Version Numbers

FCS uses its own version number sequence for assemblies and packages, approximately following SemVer rules.
To update the version number a global replace through fcs\... is currently needed, e.g.

   Directory.Build.props
   nuget/FSharp.Compiler.Service.nuspec
   nuget/FSharp.Compiler.Service.MSBuild.v12.nuspec
   nuget/FSharp.Compiler.Service.ProjectCracker.nuspec
   RELEASE_NOTES.md

## Building, Testing, Packaging, Releases

To build the package use any of:

    fcs\build Build.NetFx
    fcs\build Test.NetFx
    fcs\build NuGet.NetFx

    fcs\build Build.NetStd
    fcs\build Test.NetStd
    fcs\build NuGet.NetStd

    fcs\build Build
    fcs\build Test
    fcs\build NuGet
    fcs\build Release

which does things like:

    cd fcs
    .paket\paket.bootstrapper.exe
    .paket\paket.exe restore
    dotnet restore tools.proj
    packages\FAKE\tools\FAKE.exe build.fsx WhateverTarget

### Manual push of packages

You can push the packages if you have permissions, either automatically using ``build Release`` or manually

    set APIKEY=...
    ..\fsharp\.nuget\nuget.exe push %HOMEDRIVE%%HOMEPATH%\Downloads\FSharp.Compiler.Service.22.0.3.nupkg %APIKEY% -Source https://nuget.org 
    ..\fsharp\.nuget\nuget.exe push %HOMEDRIVE%%HOMEPATH%\Downloads\FSharp.Compiler.Service.MSBuild.v12.22.0.3.nupkg %APIKEY%  -Source https://nuget.org
    ..\fsharp\.nuget\nuget.exe push %HOMEDRIVE%%HOMEPATH%\Downloads\FSharp.Compiler.Service.ProjectCracker.22.0.3.nupkg %APIKEY%  -Source https://nuget.org
    

### Use of Paket and FAKE

Paket is only used to get FAKE and FSharp.Formatting tools.  Eventually we will likely remove this once we update the project files to .NET SDK 2.0.

FAKE is only used to run build.fsx.  Eventually we will likely remove this once we update the project files to .NET SDK 2.0.

### Testing

Testing reuses the test files from ..\tests\service which were are also FCS tests. 


### Documentation Generation

    fcs\build GenerateDocs

Output is in ``docs``.  In the ``FSharp.Compiler.Service`` repo this is checked in and hosted as http://fsharp.github.io/FSharp.Compiler.Service.


## The two other NuGet packages

It also contains both the source, build, packaging and test logic for 

* ``FSharp.Compiler.Service.MSBuild.v12`` adds legacy MSBuild v12 support to an instance of FSharp.Compiler.Service, if exact compatibility for scripting references such as ``#r "Foo, Version=1.3.4"`` is required. 

* ``FSharp.Compiler.Service.ProjectCracker`` is part of ``FsAutoComplete`` and Ionide and is used to crack old-style project formats using MSBuild. It used to be part of the FCS API.

Both of these components are gradually becoming obsolete

## Engineering road map

FSharp.Compiler.Service is a somewhat awkward component. There are some things we can do to simplify things:

1. Remove the use of Paket and FAKE
1. Move all projects under fcs\... to new .NET SDK project file format 
1. Drop the use of ``dotnet mergenupkg`` since we should be able to use cross targeting
1. Make FCS a DLL similar ot the rest of the build and make this an official component from Microsoft (signed etc.)
1. Replace FSharp.Compiler.Private by FSharp.Compiler.Service

