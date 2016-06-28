# F# Compiler, Core Library and Visual F# Tools Open Contribution Repository

This repo is where you can contribute to the F# compiler, core library and the Visual F# Tools.
To learn what F# is and why it's interesting, go to [fsharp.org](http://fsharp.org). To get a free F# environment, go to [fsharp.org](http://fsharp.org/use/windows).

**Compiler Technical Documentation**

The primary technical documents for the F# compiler code are

* [The F# Language Specification](http://fsharp.org/specs/language-spec/)

* [The F# Compiler Technical Guide](http://fsharp.github.io/2015/09/29/fsharp-compiler-guide.html) 
  maintained by contributors to this repository.  Please read
  and contribute to that guide.

**License**
> Contributions made to this repo are subject to terms and conditions of the Apache License, Version 2.0. A copy of the license can be found in the [License.txt](License.txt) file at the root of this distribution.
> By using this source code in any fashion, you are agreeing to be bound by the terms of the Apache License, Version 2.0. You must not remove this notice, or any other, from this software.

**Questions?** If you have questions about the source code, please ask in the issues.

## Quick Start: Build, Test, Develop

You can build the compiler+tools and run the subset the tests used for continuous integration as follows:

    build.cmd

There are various qualifiers:

    build.cmd release         -- build release (the default)
    build.cmd debug           -- build debug instead of release

    build.cmd proto           -- force the rebuild of the Proto bootstrap compiler in addition to other things

    build.cmd coreclr         -- build/tests only the coreclr version compiler (not the Visual F# IDE Tools)
    build.cmd compiler        -- build/tests only the compiler (not the Visual F# IDE Tools)
    build.cmd vs              -- build/tests the Visual F# IDE Tools
    build.cmd pcls            -- build/tests the PCL FSharp.Core libraries

    build.cmd build           -- build, do not test
    build.cmd ci              -- build, run the same tests as CI 
    build.cmd all             -- build, run all tests
    build.cmd notests         -- turn off testing (used in conjunction with other options)

    build.cmd test-smoke      -- build, run smoke tests
    build.cmd test-coreunit   -- build, run FSharp.Core tests
    build.cmd test-coreclr    -- build, run CoreCLR tests
    build.cmd test-pcls       -- build, run PCL tests
    build.cmd test-fsharp     -- build, run tests\fsharp suite
    build.cmd test-fsharpqa   -- build, run tests\fsharpqa suite
    build.cmd test-vs         -- build, run Visual F# IDE Tools unit tests

Combinations are also allowed:

    build.cmd debug,compiler,notests   -- build the debug compiler and run smoke tests

After you build the first time you can open and use this solution:

    .\VisualFSharp.sln

or just build it directly:

    msbuild VisualFSharp.sln 

Building ``VisualFSharp.sln`` builds _nearly_ everything. However building portable profiles of 
FSharp.Core.dll is not included.  If you are just developing the core compiler, library
and Visual F# Tools then building the solution will be enough.

## Step by Step: 

### 1. Building a Proto Compiler

The compiler is compiled as a set of .NET 4.0 components using a bootstrap process. 
This uses a Last Known Good (LKG) compiler checked into this repository to build.  

    msbuild src\fsharp-proto-build.proj
    
### 2.  Building an F# (Debug) library and compiler

This uses the proto compiler to build `FSharp.Core.dll`, `FSharp.Compiler.dll`, `fsc.exe`, and `fsi.exe`.

    msbuild src/fsharp-library-build.proj 
    msbuild src/fsharp-compiler-build.proj 
    
You can now use the updated F# compiler in `debug\net40\bin\fsc.exe` and F# Interactive in `debug\net40\bin\fsi.exe` to develop and test basic language and tool features.

**Note:** The updated library is not used until you run `update.cmd`, see below.  The updated compiler is not run 'pre-compiled' until you run `update.cmd -ngen`, see below.

### 3. Full Steps Before Running Tests

See [TESTGUIDE.md](TESTGUIDE.md) for full details on how to run tests.
    
Prior to a full **Debug** test run, you need to complete **all** of the steps in build.cmd

    build.cmd debug,build

Likewise prior to a **Release** test run:

    build.cmd release,build

For **Debug** this corresponds to these steps, which you can run individually for more incremental builds:

    msbuild src/fsharp-library-build.proj
    msbuild src/fsharp-compiler-build.proj
    msbuild src/fsharp-compiler-unittests-build.proj
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable47
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable7
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable78
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable259
    msbuild src/fsharp-library-unittests-build.proj
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable259
    msbuild vsintegration/fsharp-vsintegration-src-build.proj
    msbuild vsintegration/fsharp-vsintegration-project-templates-build.proj
    msbuild vsintegration/fsharp-vsintegration-item-templates-build.proj
    msbuild vsintegration/fsharp-vsintegration-deployment-build.proj
    msbuild vsintegration/fsharp-vsintegration-unittests-build.proj 
    msbuild tests/fsharp/FSharp.Tests.fsproj
    src\update.cmd debug -ngen
    tests\BuildTestTools.cmd debug 


For **Release** this corresponds to these steps, which you can run individually for more incremental builds:

    msbuild src/fsharp-library-build.proj  /p:Configuration=Release
    msbuild src/fsharp-compiler-build.proj  /p:Configuration=Release
    msbuild src/fsharp-compiler-unittests-build.proj  /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable47 /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable7 /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable78 /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable259 /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj  /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47 /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7 /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78 /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable259 /p:Configuration=Release
    msbuild vsintegration/fsharp-vsintegration-src-build.proj /p:Configuration=Release
    msbuild vsintegration/fsharp-vsintegration-project-templates-build.proj /p:Configuration=Release
    msbuild vsintegration/fsharp-vsintegration-item-templates-build.proj /p:Configuration=Release
    msbuild vsintegration/fsharp-vsintegration-deployment-build.proj /p:Configuration=Release
    msbuild vsintegration/fsharp-vsintegration-unittests-build.proj  /p:Configuration=Release
    msbuild tests/fsharp/FSharp.Tests.fsproj /p:Configuration=Release
    src\update.cmd release -ngen
    tests\BuildTestTools.cmd release 

### 4. [Optional] Install the Visual F# IDE Tools 

**Note:** This step will install a VSIX extension into Visual Studio 15 that changes the Visual F# IDE Tools 
components installed into Visual Studio 15.  You can revert this step by disabling or uninstalling the addin.

For **Debug**:

1. Ensure that the VSIX package is uninstalled. In VS, select Tools/Extensions and Updates and if the package `Visual F# Tools` is installed, select Uninstall
1. Run ``debug\net40\bin\VisualFSharpVsix.vsix``

For **Release**:

1. Ensure that the VSIX package is uninstalled. In VS, select Tools/Extensions and Updates and if the package `Visual F# Tools` is installed, select Uninstall
1. Run ``release\net40\bin\VisualFSharpVsix.vsix``

Restart Visual Studio, it should now be running your freshly-built Visual F# IDE Tools with updated F# Interactive. 

### 5. [Optional] Clobber the F# SDK on the machine

**Note:** Step #3 below will clobber the machine-wide installed F# SDK on your machine. This replaces the ``fsi.exe``/``fsiAnyCpu.exe`` used by Visual F# Interactive and the ``fsc.exe`` used by ``Microsoft.FSharp.targets``.  Repairing Visual Studio 15 is currently the only way to revert this step.  

For **Debug**:

1. Run ``vsintegration\update-vsintegration.cmd debug`` (clobbers the installed F# SDK)

For **Release**:

1. Run ``vsintegration\update-vsintegration.cmd release`` (clobbers the installed F# SDK)

### Notes on the build

1. The `update.cmd` script adds required strong name validation skips, and NGens the compiler and libraries. This requires admin privileges.
1. The compiler binaries produced are "private" and strong-named signed with a test key.
1. Some additional tools are required to build the compiler, notably `fslex.exe`, `fsyacc.exe`, `FSharp.PowerPack.Build.Tasks.dll`, `FsSrGen.exe`, `FSharp.SRGen.Build.Tasks.dll`, and the other tools found in the `lkg` directory.
1. The overall bootstrapping process executes as follows
 - We first need an existing F# compiler. We use the one in the `lkg` directory. Let's assume this compiler has an `FSharp.Core.dll` with version X.
 - We use this compiler to compile the source in this distribution, to produce a "proto" compiler, dropped to the `proto` directory. When run, this compiler still relies on `FSharp.Core.dll` with version X.
 - We use the proto compiler to compile the source for `FSharp.Core.dll` in this distribution.
 - We use the proto compiler to compile the source for `FSharp.Compiler.dll`, `fsc.exe`, `fsi.exe`, and other binaries found in this distribution.

## Resources

The primary technical guide to the core compiler code is [The F# Compiler Technical Guide](http://fsharp.github.io/2015/09/29/fsharp-compiler-guide.html).  Please read and contribute to that guide.

