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

## 0.  A Shortcut to Build and Smoke Test

You can build a subset of functionality (including bootstrapped compiler and library) and run a very 
small number of 'smoke' tests using the script used by continuous integration on Windows:

    .\appveyor-build.cmd

See the script for what this does.  After you do this, you can do further testing, see  [TESTGUIDE.md](TESTGUIDE.md).


## 1.  Building a Proto Compiler

The compiler is compiled as a set of .NET 4.0 components using a bootstrap process. This uses the Last Known Good (LKG) compiler to build.  
Note that you need the .NET framework 3.5 installed on your machine in order to complete this step.

    msbuild src\fsharp-proto-build.proj
    
## 2.  Building an F# (Debug) library and compiler

This uses the proto compiler to build `FSharp.Core.dll`, `FSharp.Compiler.dll`, `fsc.exe`, and `fsi.exe`.

    msbuild src/fsharp-library-build.proj 
    msbuild src/fsharp-compiler-build.proj 
    
You can now use the updated F# compiler in `debug\net40\bin\fsc.exe` and F# Interactive in `debug\net40\bin\fsi.exe` to develop and test basic language and tool features.

**Note:** The updated library is not used until you run `update.cmd`, see below.  The updated compiler is not run 'pre-compiled' until you run `update.cmd -ngen`, see below.

## 3. Full Steps Before Running Tests

See [TESTGUIDE.md](TESTGUIDE.md) for full details on how to run tests.
    
Prior to a **Debug** test run, you need to complete **all** of these steps:

    msbuild src/fsharp-library-build.proj
    msbuild src/fsharp-compiler-build.proj
    msbuild src/fsharp-typeproviders-build.proj
    msbuild src/fsharp-compiler-unittests-build.proj
    msbuild src/fsharp-library-build.proj /p:TargetFramework=net20
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable47
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable7
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable78
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable259
    msbuild src/fsharp-library-unittests-build.proj
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable259
    src\update.cmd debug -ngen
    tests\BuildTestTools.cmd debug 


[Optional] If testing the Visual Studio bits (see below) you will also need:

    msbuild vsintegration\fsharp-vsintegration-build.proj
    msbuild vsintegration\fsharp-vsintegration-unittests-build.proj

Prior to a **Release** test run, you need to do **all** of these:

    msbuild src/fsharp-library-build.proj  /p:Configuration=Release
    msbuild src/fsharp-compiler-build.proj  /p:Configuration=Release
    msbuild src/fsharp-typeproviders-build.proj  /p:Configuration=Release
    msbuild src/fsharp-compiler-unittests-build.proj  /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=net20 /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable47 /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable7 /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable78 /p:Configuration=Release
    msbuild src/fsharp-library-build.proj /p:TargetFramework=portable259 /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj  /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47 /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7 /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78 /p:Configuration=Release
    msbuild src/fsharp-library-unittests-build.proj /p:TargetFramework=portable259 /p:Configuration=Release
    src\update.cmd release -ngen
    tests\BuildTestTools.cmd release 


[Optional] If testing **Release** build of the Visual F# IDE Tools (see below) you will also need:

    msbuild vsintegration\fsharp-vsintegration-build.proj /p:Configuration=Release
    msbuild vsintegration\fsharp-vsintegration-unittests-build.proj /p:Configuration=Release

## 4. [Optional] Install the Visual F# IDE Tools and Clobber the F# SDK on the machine

**Note:** Step #2 below will install a VSIX extension into Visual Studio 2015 that changes the Visual F# IDE Tools 
components installed into Visual Studio 2015.  You can revert this step by disabling or uninstalling the addin.

**Note:** Step #3 below will clobber the machine-wide installed F# SDK on your machine. This replaces the ``fsi.exe``/``fsiAnyCpu.exe`` used 
by Visual F# Interactive and the ``fsc.exe`` used by ``Microsoft.FSharp.targets``.  Repairing Visual Studio 2015 is currently the 
only way to revert this step.  

**Note:** After you complete the install, the FSharp.Core referenced by your projects will not be updated. If you want to make
a project that references your updated FSharp.Core, you must explicitly change the ``TargetFSharpCoreVersion`` in the .fsproj
file to ``4.4.0.5099`` (or a corresponding portable version number with suffix ``5099``).

For **Debug**:

1. Ensure that the VSIX package is uninstalled. In VS, select Tools/Extensions and Updates and if the package `VisualStudio.FSharp.EnableOpenSource` is installed, select Uninstall
1. Run ``debug\net40\bin\EnableOpenSource.vsix``
1. Run ``vsintegration\update-vsintegration.cmd debug`` (clobbers the installed F# SDK)

For **Release**:

1. Ensure that the VSIX package is uninstalled. In VS, select Tools/Extensions and Updates and if the package `VisualStudio.FSharp.EnableOpenSource` is installed, select Uninstall
1. Run ``release\net40\bin\EnableOpenSource.vsix``
1. Run ``vsintegration\update-vsintegration.cmd release`` (clobbers the installed F# SDK)

Restart Visual Studio, it should now be running your freshly-built Visual F# IDE Tools with updated F# Interactive. 


### Notes on the build

1. The `update.cmd` script adds the built `FSharp.Core` to the GAC, adds required strong name validation skips, and NGens the compiler and libraries. This requires admin privileges.
1. The compiler binaries produced are "private" and strong-named signed with a test key.
1. Some additional tools are required to build the compiler, notably `fslex.exe`, `fsyacc.exe`, `FSharp.PowerPack.Build.Tasks.dll`, `FsSrGen.exe`, `FSharp.SRGen.Build.Tasks.dll`, and the other tools found in the `lkg` directory.
1. The overall bootstrapping process executes as follows
 - We first need an existing F# compiler. We use the one in the `lkg` directory. Let's assume this compiler has an `FSharp.Core.dll` with version X.
 - We use this compiler to compile the source in this distribution, to produce a "proto" compiler, dropped to the `proto` directory. When run, this compiler still relies on `FSharp.Core.dll` with version X.
 - We use the proto compiler to compile the source for `FSharp.Core.dll` in this distribution.
 - We use the proto compiler to compile the source for `FSharp.Compiler.dll`, `fsc.exe`, `fsi.exe`, and other binaries found in this distribution.

### Further technical resources

The primary technical guide to the core compiler code is [The F# Compiler Technical Guide](http://fsharp.github.io/2015/09/29/fsharp-compiler-guide.html).  Please read and contribute to that guide.

