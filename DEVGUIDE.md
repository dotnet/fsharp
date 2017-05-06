# Development Guide

Follow the instructions below to build and develop the F# Compiler, Core Library and tools on Windows, macOS and Linux.

* [Developing the F# Compiler (Windows)](#developing-the-f-compiler-windows)
* [Developing the F# Compiler (Linux)](#developing-the-f-compiler-linux)
* [Developing the F# Compiler (macOS)](#developing-the-f-compiler-macos)
* [Developing the Visual F# IDE Tools (Windows Only)](#developing-the-visual-f-ide-tools-windows-only) 
* [Notes and Resources](#notes)

###  Developing the F# Compiler (Windows)

Install

- [.NET 4.5.1](http://www.microsoft.com/en-us/download/details.aspx?id=40779)
- [MSBuild 12.0](http://www.microsoft.com/en-us/download/details.aspx?id=40760)

On Windows you can build the F# compiler for .NET Framework as follows:

    build.cmd

This is the same as

    build.cmd net40

There are various qualifiers:

    build.cmd release         -- build release (the default)
    build.cmd debug           -- build debug instead of release

    build.cmd net40           -- build .NET Framework compiler (the default)
    build.cmd coreclr         -- build .NET Core compiler 
    build.cmd vs              -- build the Visual F# IDE Tools (see below)
    build.cmd pcls            -- build the PCL FSharp.Core libraries
    build.cmd all             -- build all 

    build.cmd proto           -- force the rebuild of the Proto bootstrap compiler in addition to other things

    build.cmd test            -- build default targets, run suitable tests
    build.cmd net40 test      -- build net40, run suitable tests
    build.cmd coreclr test    -- build coreclr, run suitable tests
    build.cmd vs test         -- build Visual F# IDE Tools, run all tests (see below)
    build.cmd all test        -- build all, run all tests

    build.cmd test-smoke      -- build, run smoke tests
    build.cmd test-net40-fsharp     -- build, run tests\fsharp suite for .NET Framework
    build.cmd test-net40-fsharpqa   -- build, run tests\fsharpqa suite for .NET Framework

After you build the first time you can open and use this solution:

    .\FSharp.sln

or just build it directly:

    msbuild FSharp.sln 

Building ``FSharp.sln`` builds nearly everything. However building portable profiles of 
FSharp.Core.dll is not included.  If you are just developing the core compiler and library
then building the solution will be enough.

###  Developing the F# Compiler (Linux)

For Linux/Mono, follow [these instructions](http://www.mono-project.com/docs/getting-started/install/linux/). Also you may need:

    sudo apt-get install mono-complete autoconf libtool pkg-config make git automake

Then:
    
    ./autoconf.sh --prefix /usr
    make
    make install

Full testing is not yet enabled on Linux, nor is a .NET Core build of the compiler.

You can alternatively use

    ./build.sh

###  Developing the F# Compiler (macOS)

Install Xamarin Studio, then

    ./autogen.sh --prefix=/Library/Frameworks/Mono.framework/Versions/Current/
    make
    sudo make install

### Developing the Visual F# IDE Tools (Windows Only)

To build and test Visual F# IDE Tools, install these requirements:
- [Visual Studio 2017](https://www.visualstudio.com/downloads/)
  - Under the "Windows" workloads, select ".NET desktop development"
    - Select "F# language support" under the optional components
  - Under the "Other Toolsets" workloads, select "Visual Studio extension development"
  - Under the "Individual components" tab select "Windows 10 SDK" as shown below (needed for compiling RC resource, see #2556): \
  ![image](https://cloud.githubusercontent.com/assets/1249087/23730261/5c78c850-041b-11e7-9d9d-62766351fd0f.png)

Steps to build:

    build.cmd vs              -- build the Visual F# IDE Tools in Release configuration (see below)
    build.cmd vs debug        -- build the Visual F# IDE Tools in Debug configuration (see below)
    build.cmd vs test         -- build Visual F# IDE Tools, run all tests (see below)

Use ``VisualFSharp.sln`` if you're building the Visual F# IDE Tools.


Note on Debug vs Release: ``Release`` Configuration has a degraded debugging experience, so if you want to test a change locally, it is recommended to do it in the ``Debug`` configuration. For more information see https://github.com/Microsoft/visualfsharp/issues/2771 and https://github.com/Microsoft/visualfsharp/pull/2773.

Note: if you face this error [#2351](https://github.com/Microsoft/visualfsharp/issues/2351):

>  error VSSDK1077: Unable to locate the extensions directory. "ExternalSettingsManager::GetScopePaths failed to initialize PkgDefManager for C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe".

Or hard crash on launch ("Unknown Error"), delete these folders:

* `%localappdata%\Microsoft\VisualStudio\15.0_(some number here)FSharpDev`
* `%localappdata%\Microsoft\VisualStudio\15.0_(some number here)`

#### [Optional] Install the Visual F# IDE Tools  (Windows Only)

At time of writing, the Visual F# IDE Tools can only be installed into the latest Visual Studio 2017 RC releases.
The new builds of the Visual F# IDE Tools can no longer be installed into Visual Studio 2015.

You can install Visual Studio 2017 from https://www.visualstudio.com/downloads/.

**Note:** This step will install a VSIX extension into Visual Studio "Next" that changes the Visual F# IDE Tools 
components installed in that VS installation.  You can revert this step by disabling or uninstalling the addin.

For **Debug**, uninstall then reinstall:

    VSIXInstaller.exe /u:"VisualFSharp"
    VSIXInstaller.exe debug\net40\bin\VisualFSharpOpenSource.vsix

For **Release**, uninstall then reinstall:

    VSIXInstaller.exe /u:"VisualFSharp"
    VSIXInstaller.exe release\net40\bin\VisualFSharpOpenSource.vsix

Restart Visual Studio, it should now be running your freshly-built Visual F# IDE Tools with updated F# Interactive.

#### [Optional] F5 testing of local changes

To test your changes locally _without_ overwriting your default installed F# tools, set the `VisualFSharp\Vsix\VisualFSharpOpenSource`
project as the startup project.  When you hit F5 a new instance of Visual Studio will be started in the `FSharpDev` hive with your
changes, but the root (default) hive will remain untouched.

#### [Optional] Rapid deployment of incremental changes to Visual F# IDE Tools components

For the brave, you can rapidly deploy incrementally updated versions of Visual F# IDE Tool components such as ``FSHarp.Editor.dll`` by copying them directly into the extension directory in your user AppData folder:

    xcopy /y debug\net40\bin\FSharp.* "%USERPROFILE%\AppData\Local\Microsoft\VisualStudio\15.0_7c5620b7FSharpDev\Extensions\Microsoft.VisualFSharpTools\Visual F# Tools\15.4.1.9055"

This gives a much tighter inner development loop than uninstalling/reinstalling the VSIX, as you do not have to restart VIsual Studio. Caveat emptor.

#### [Optional] Clobber the F# SDK on the machine

**Note:** The step below will try to clobber the machine-wide installed F# SDK on your machine. This replaces the ``fsc.exe`` used by the standard innstall location or ``Microsoft.FSharp.targets``.  **Repairing Visual Studio 15 is currently the only way to revert this step.**

For **Debug**:

    vsintegration\update-vsintegration.cmd debug

For **Release**:

    vsintegration\update-vsintegration.cmd release


# Notes

#### Windows: Links to  Additional frameworks

- [Git for windows](http://msysgit.github.io/)
- [.NET 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21)
- [.NET 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=30653)
- [.NET 4.5.1](http://www.microsoft.com/en-us/download/details.aspx?id=40779)
- [.NET 4.6](http://www.microsoft.com/en-us/download/details.aspx?id=48137)
- [MSBuild 12.0](http://www.microsoft.com/en-us/download/details.aspx?id=40760)
- [Windows 7 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=8279)
- [Windows 8 SDK](http://msdn.microsoft.com/en-us/windows/desktop/hh852363.aspx)
- [Windows 8.1 SDK](http://msdn.microsoft.com/en-us/library/windows/desktop/bg162891.aspx)
- [Windows 10 SDK](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk)


#### Notes on the Windows .NET Framework build

1. The `update.cmd` script adds required strong name validation skips, and NGens the compiler and libraries. This requires admin privileges.
1. The compiler binaries produced are "private" and strong-named signed with a test key.
1. Some additional tools are required to build the compiler, notably `fslex.exe`, `fsyacc.exe`, `FSharp.PowerPack.Build.Tasks.dll`, `FsSrGen.exe`, `FSharp.SRGen.Build.Tasks.dll`, and the other tools found in the `lkg` directory.
1. The overall bootstrapping process executes as follows
 - We first need an existing F# compiler. We use the one in the `lkg` directory. Let's assume this compiler has an `FSharp.Core.dll` with version X.
 - We use this compiler to compile the source in this distribution, to produce a "proto" compiler, dropped to the `proto` directory. When run, this compiler still relies on `FSharp.Core.dll` with version X.
 - We use the proto compiler to compile the source for `FSharp.Core.dll` in this distribution.
 - We use the proto compiler to compile the source for `FSharp.Compiler.dll`, `fsc.exe`, `fsi.exe`, and other binaries found in this distribution.

#### Configuring proxy server

If you are behind a proxy server, NuGet client tool must be configured to use it:

    .nuget\nuget.exe config -set http_proxy=proxy.domain.com:8080 -ConfigFile .nuget\NuGet.Config
    .nuget\nuget.exe config -set http_proxy.user=user_name -ConfigFile .nuget\NuGet.Config
    .nuget\nuget.exe config -set http_proxy.password=user_password -ConfigFile .nuget\NuGet.Config

Where you should set proper proxy address, user name and password.

#### Resources

The primary technical guide to the core compiler code is [The F# Compiler Technical Guide](http://fsharp.github.io/2015/09/29/fsharp-compiler-guide.html).  Please read and contribute to that guide.

