

## The FSharp.Compiler.Service components and nuget package

This directory contains the build, packaging, test and documentation-generation logic for the nuget package ``FSharp.Compiler.Service``.  The source for this nuget
package is in ``..\src``.

## FSharp.Compiler.Service v. FSharp.Compiler.Private

There are subtle differences between FSharp.Compiler.Service and FSharp.Compiler.Private

- Public API 
- Built against .NET 4.5 to give broader reach
- Built against FSharp.Core 4.0.0.0 to give broader reach
- Has .NET Standard 1.6 version 

## Version Numbers

FCS uses its own version number sequence for assemblies and packages, approximately following SemVer rules.
To update the version number a global replace through fcs\... is currently needed, e.g.

   fcs.props
   nuget/FSharp.Compiler.Service.nuspec
   nuget/FSharp.Compiler.Service.MSBuild.v12.nuspec
   nuget/FSharp.Compiler.Service.ProjectCracker.nuspec
   RELEASE_NOTES.md

## Building

To build the package use any of:

  build 
  build Build
  build Nuget
  build Release


which does

  .paket\paket.bootstrapper.exe
  .paket\paket.exe restore
  packages\FAKE\tools\FAKE.exe build.fsx WhateverTarget

### Testing

Testing reuses the test files from ..\tests\service which were originally intended as FCS tests. Test using

    build RunTests.NetFx
    build RunTests.NetCore


### Documentation Generation

Use

    build GenerateDocs


### Long term plans

This part of this repo uses FAKE and paket for historical reasons.  There are some things we can do to simplify things:

1. Move to NUnit 3.x (same as rest of repo)
1. Remove the use of Paket and just use the other packages already restored
1. Move to new .NET SDK project file format 
1. Drop the explicit code generation for the .NET Core package and use standard FsLexYacc.targets etc
1. Drop the use of ``dotnet mergenupkg`` since we should be able to use cross targeting

Eventually we may unify this part of the repo with the rest of the build and make this an official component.


## The two other nuget packages

It also contains both the source, build, packaging and test logic for  the nuget packages ``FSharp.Compiler.Service.MSBuild.v12`` and
``FSharp.Compiler.Service.ProjectCracker``.  Both of these components are gradually becoming obsolete and are now more rarely used.
The first adds legacy MSBuild v12 support to an instance of FSharp.Compiler.Service, if exact compatibility for
scripting references such as ``#r "Foo, Version=1.3.4"`` is required.  The second is part of ``FsAutoComplete`` and Ionide and is used to crack
old-style project formats.

  