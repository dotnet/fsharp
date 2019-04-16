# Development Guide

## Get the Latest F# Compiler Source Code

Get the latest source code from the master branch by running this git command:

    git clone https://github.com/Microsoft/visualfsharp.git
    
Before running the build scripts, ensure that you have cleaned up the visualfsharp repo by running this git command:

    git clean -xfd

This will remove any files that are not under version control. This is necessary only if you have already attempted to build the solution or have made other changes that might prevent it from building.

## Installing Dependencies and Building

Follow the instructions below to build and develop the F# Compiler, Core Library and tools on Windows, macOS and Linux.

- [Developing the F# Compiler (Windows)](#developing-the-f-compiler-windows)
- [Developing the F# Compiler (Linux)](#developing-the-f-compiler-linux)
- [Developing the F# Compiler (macOS)](#developing-the-f-compiler-macos)
- [Developing the Visual F# IDE Tools (Windows Only)](#developing-the-visual-f-ide-tools-windows-only)
- [Notes and Resources](#notes)

### Developing the F# Compiler (Windows)

Install

- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)

**NOTE on Windows:**

1. It is recommended to run `build.cmd` in a command prompt with path set to have the location of MSBuild. If you have Visual Studio, we can run using `Developer Command Prompt for Visual Studio 20xx` (depends on Visual Studio version). This developer command prompt is easier to use than normal command prompt, because it already has the correct path of Visual Studio and .NET's tooling set for us to use (including MSBuild).

2. The command prompt must have Administrator rights (`Run as Administrator`).

On Windows you can build the F# compiler and tools as follows:

    Build.cmd

Desktop tests can be run with:

    Build.cmd -test

Additional options are available via:

    Build.cmd /?

After you build the first time you can open and use this solution:

    .\VisualFSharp.sln

If you are just developing the core compiler and library then building ``FSharp.sln`` will be enough.

### Developing the F# Compiler (Linux/macOS)

For Linux/Mac:

    ./build.sh

Running tests:

    ./build.sh -test

### Developing the Visual F# IDE Tools (Windows Only)

To build and test Visual F# IDE Tools, install these requirements:

- Download [Visual Studio 2019](https://www.visualstudio.com/downloads/)
- Launch the Visual Studio Installer
  - Under the **"Windows"** workload, select **".NET desktop development"**
    - Select the optional component **"F# desktop language support"**
  - Under the **"Mobile & Gaming"** workload, select **"Mobile development with .NET"**
  - Under the **"Other Toolsets"** workload, select **"Visual Studio extension development"**
  - On the **"Individual Components"** tab, select **".NET Framework 4.7.2 SDK"** and **".NET Framework 4.7.2 targeting pack"**

Steps to build:

    Build.cmd                             -- build all F# components under the default configuration (Debug)
    Build.cmd -configuration Release      -- build all F# components as Release
    Build.cmd -testDesktop                -- build and test all net472 tests

All test options:

    -testDesktop                          -- test all net472 target frameworks
    -testCoreClr                          -- test all netstandard and netcoreapp target frameworks
    -testFSharpQA                         -- test all F# Cambridge tests
    -testVs                               -- test all VS integration points
    -testFcs                              -- test F# compiler service components
    -testAll                              -- all of the above

Use ``VisualFSharp.sln`` if you're building the Visual F# IDE Tools.

Note on Debug vs Release: ``Release`` Configuration has a degraded debugging experience, so if you want to test a change locally, it is recommended to do it in the ``Debug`` configuration. For more information see issues [#2771](https://github.com/Microsoft/visualfsharp/issues/2771) and [#2773](https://github.com/Microsoft/visualfsharp/pull/2773).

Note ([#2351](https://github.com/Microsoft/visualfsharp/issues/2351)): if you face this error:

> error VSSDK1077: Unable to locate the extensions directory. "ExternalSettingsManager::GetScopePaths failed to initialize PkgDefManager for C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe".

Or hard crash on launch ("Unknown Error"), delete these folders:

- `%localappdata%\Microsoft\VisualStudio\16.0_(some number here)RoslynDev`
- `%localappdata%\Microsoft\VisualStudio\16.0_(some number here)`

#### [Optional] Install the Visual F# IDE Tools  (Windows Only)

The new builds of the Visual F# IDE Tools can no longer be installed into Visual Studio 2015.

You can install Visual Studio 2019 from https://www.visualstudio.com/downloads/.

**Note:** This step will install a VSIX extension into Visual Studio "Next" that changes the Visual F# IDE Tools 
components installed in that VS installation.  You can revert this step by disabling or uninstalling the addin.

For **Debug**, uninstall then reinstall:

    VSIXInstaller.exe /u:"VisualFSharp"
    VSIXInstaller.exe artifacts\VSSetup\Debug\VisualFSharpFull.vsix

For **Release**, uninstall then reinstall:

    VSIXInstaller.exe /u:"VisualFSharp"
    VSIXInstaller.exe artifacts\VSSetup\Release\VisualFSharpFull.vsix

Restart Visual Studio, it should now be running your freshly-built Visual F# IDE Tools with updated F# Interactive.

#### [Optional] F5 testing of local changes

To test your changes locally _without_ overwriting your default installed Visual F# tools, set the `VisualFSharp\Vsix\VisualFSharpFull`
project as the startup project.  When you hit F5 a new instance of Visual Studio will be started in the `RoslynDev` hive with your
changes, but the root (default) hive will remain untouched. You can also start this hive automatically using

    devenv.exe /rootsuffix RoslynDev

Because this uses the "RoslynDev" hive you can simultaneously test changes to an appropriate build of Roslyn binaries.

#### [Optional] Rapid deployment of incremental changes to Visual F# IDE Tools components

For the brave, you can rapidly deploy incrementally updated versions of Visual F# IDE Tool components such as ``FSharp.Editor.dll`` by copying them directly into the extension directory in your user AppData folder:

    xcopy /y debug\net40\bin\FSharp.* "%USERPROFILE%\AppData\Local\Microsoft\VisualStudio\16.0_7c5620b7FSharpDev\Extensions\Microsoft.VisualFSharpTools\Visual F# Tools\16.4.1.9055"

This gives a much tighter inner development loop than uninstalling/reinstalling the VSIX, as you do not have to restart VIsual Studio. Caveat emptor.

#### [Optional] Clobber the F# SDK on the machine

**Note:** The step below will try to clobber the machine-wide installed F# SDK on your machine. This replaces the ``fsc.exe`` used by the standard install location or ``Microsoft.FSharp.Targets``.  **Repairing Visual Studio 16 is currently the only way to revert this step.**

For **Debug**:

    vsintegration\update-vsintegration.cmd debug

For **Release**:

    vsintegration\update-vsintegration.cmd release

## Debugging the F# Compiler

See the "Debugging The Compiler" section of this [article](https://medium.com/@willie.tetlow/f-mentorship-week-1-36f51d3812d4)

## Notes

#### Windows: Links to  Additional frameworks

- [Git for windows](http://msysgit.github.io/)
- [.NET 4.6](http://www.microsoft.com/en-us/download/details.aspx?id=48137)
- [Windows 8.1 SDK](http://msdn.microsoft.com/en-us/library/windows/desktop/bg162891.aspx)
- [Windows 10 SDK](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk)

#### Notes on the Windows .NET Framework build

1. The `update.cmd` script adds required strong name validation skips and NGens the compiler and libraries. This requires admin privileges.
1. The compiler binaries produced are "private" and strong-named signed with a test key.
1. Some additional tools are required to build the compiler, notably `fslex.exe`, `fsyacc.exe`, `FSharp.PowerPack.Build.Tasks.dll`, `FsSrGen.exe`, `FSharp.SRGen.Build.Tasks.dll`, and the other tools found in the `lkg` directory.
1. The overall bootstrapping process executes as follows
 - We first need an existing F# compiler. We use the one in the `lkg` directory. Let's assume this compiler has an `FSharp.Core.dll` with version X.
 - We use this compiler to compile the source in this distribution, to produce a "proto" compiler, dropped to the `proto` directory. When run, this compiler still relies on `FSharp.Core.dll` with version X.
 - We use the proto compiler to compile the source for `FSharp.Core.dll` in this distribution.
 - We use the proto compiler to compile the source for `FSharp.Compiler.dll`, `fsc.exe`, `fsi.exe`, and other binaries found in this distribution.

#### Updating FSComp.fs, FSComp.resx and XLF

If your changes involve modifying the list of language keywords in any way, (e.g. when implementing a new keyword), the XLF localization files need to be synced with the corresponding resx files. This can be done automatically by running

    pushd src\fsharp\FSharp.Compiler.Private
    msbuild FSharp.Compiler.Private.fsproj /t:UpdateXlf
    popd

This only works on Windows/.NETStandard framework, so changing this from any other platform requires editing and syncing all of the XLF files manually.

#### Configuring proxy server

If you are behind a proxy server, NuGet client tool must be configured to use it:

    .nuget\nuget.exe config -set http_proxy=proxy.domain.com:8080 -ConfigFile NuGet.Config
    .nuget\nuget.exe config -set http_proxy.user=user_name -ConfigFile NuGet.Config
    .nuget\nuget.exe config -set http_proxy.password=user_password -ConfigFile NuGet.Config

Where you should set proper proxy address, user name and password.

#### Resources

The primary technical guide to the core compiler code is [The F# Compiler Technical Guide](http://fsharp.github.io/2015/09/29/fsharp-compiler-guide.html).  Please read and contribute to that guide.
