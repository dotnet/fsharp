# Development Guide

This document details more advanced options for developing in this codebase. It is not quite necessary to follow it, but it is likely that you'll find something you'll need from here.

## Documentation

The compiler is documented in [docs](docs/index.md). This is essential reading.

## Recommended workflow

We recommend the following overall workflow when developing for this repository:

* Fork this repository
* Always work in your fork
* Always keep your fork up to date

Before updating your fork, run this command:

```shell
git remote add upstream https://github.com/dotnet/fsharp.git
```

This will make management of multiple forks and your own work easier over time.

## Updating your fork

We recommend the following commands to update your fork:

```shell
git checkout main
git clean -xdf
git fetch upstream
git rebase upstream/main
git push
```

Or more succinctly:

```shell
git checkout main && git clean -xdf && git fetch upstream && git rebase upstream/main && git push
```

This will update your fork with the latest from `dotnet/fsharp` on your machine and push those updates to your remote fork.

## Developing on Windows

Install the latest released [Visual Studio](https://visualstudio.microsoft.com/vs/preview/) preview, as that is what the `main` branch's tools are synced with. Select the following workloads:

* .NET desktop development (also check F# desktop support, as this will install some legacy templates)
* Visual Studio extension development

You will also need .NET SDK installed from [here](https://dotnet.microsoft.com/download/dotnet), exact version can be found in the global.json file in the root of the repository.

Building is simple:

```shell
build.cmd
```

Desktop tests can be run with:

```shell
build.cmd -test -c Release
```

After you build the first time you can open and use this solution in Visual Studio:

```shell
.\VisualFSharp.sln
```

If you don't have everything installed yet, you'll get prompted by Visual Studio to install a few more things. This is because we use a `.vsconfig` file that specifies all our dependencies.

If you are just developing the core compiler and library then building ``FSharp.sln`` will be enough.

We recommend installing the latest Visual Studio preview and using that if you are on Windows. However, if you prefer not to do that, you will need to install the following:

* [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)
* [.NET SDK](https://dotnet.microsoft.com/download/dotnet) (see exact version in global.json file in the repository root).

You'll need to pass an additional flag to the build script:

```shell
build.cmd -noVisualStudio
```

You can open `FSharp.sln` in your editor of choice.

## Developing on Linux or macOS

For Linux/Mac:

```shell
./build.sh
```

Running tests:

```shell
./build.sh --test
```

You can then open `FSharp.sln` in your editor of choice.

## Testing from the command line

You can find all test options as separate flags. For example `build -testAll`:

```shell
  -testAll                  Run all tests
  -testAllButIntegration    Run all but integration tests
  -testCambridge            Run Cambridge tests
  -testCompiler             Run FSharpCompiler unit tests
  -testCompilerService      Run FSharpCompilerService unit tests
  -testDesktop              Run tests against full .NET Framework
  -testCoreClr              Run tests against CoreCLR
  -testFSharpCore           Run FSharpCore unit tests
  -testFSharpQA             Run F# Cambridge tests
  -testScripting            Run Scripting tests
  -testVs                   Run F# editor unit tests
```

Running any of the above will build the latest changes and run tests against them.

## Using your custom compiler to build this repository

By removing all the subfolders called `Bootstrap` or `Proto` under `artifacts` and running the `build` script again, the proto compiler will include your changes.

Once the "proto" compiler is built, it won't be built again, so you may want to perform those steps again to ensure your changes don't break building the compiler itself.

## Using your custom compiler to build other projects

Building the compiler using `build.cmd` or `build.sh` will output artifacts in `artifacts\bin`.

To use your custom build of `Fsc`, add the `DotnetFscCompilerPath` property to your project's `.fsproj` file, adjusted to point at your local build directory, build configuration, and target framework as appropriate:

```xml
<PropertyGroup>
    <DotnetFscCompilerPath>D:\Git\fsharp\artifacts\bin\fsc\Debug\net9.0\fsc.dll</DotnetFscCompilerPath>
</PropertyGroup>
```

### Changes in FSharp.Core

The FSharp compiler uses an implicit FSharp.Core. This means that if you introduce changes to FSharp.Core and want to use it in a project, you need to disable the implicit version used by the compiler, and add a reference to your custom FSharp.Core dll. Both are done in the `.fsproj` file of your project.

Disabling the implicit FSharp.Core is done with
```
  <PropertyGroup>
    <DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>
  </PropertyGroup>
```
and referencing your custom FSharp.Core, available after you build the compiler, is done with
```
  <ItemGroup>
    <Reference Include="FSharp.Core">
      <HintPath>D:\Git\fsharp\artifacts\bin\FSharp.Core\Debug\netstandard2.1\FSharp.Core.dll<\HintPath>
    </Reference>
  </ItemGroup>
```

## Updating FSComp.fs, FSComp.resx and XLF

If your changes involve modifying the list of language keywords in any way, (e.g. when implementing a new keyword), the XLF localization files need to be synced with the corresponding resx files. This can be done automatically by running

```shell
dotnet build src\Compiler /t:UpdateXlf
```
If you are on a Mac, you can run this command from the root of the repository:

```shell
sh build.sh -c Release
```

Or if you are on Linux:

```shell
./build.sh -c Release
```

## Updating baselines in tests

Some tests use "baseline" (.bsl) files.  There is sometimes a way to update these baselines en-masse in your local build,
useful when some change affects many baselines.  For example, in the `fsharpqa` and `FSharp.Compiler.ComponentTests` tests the baselines
are updated using scripts or utilities that allow the following environment variable to be set:

Windows:

CMD:

```shell
set TEST_UPDATE_BSL=1
```

PowerShell:

```shell
$env:TEST_UPDATE_BSL=1
```

Linux/macOS:

```shell
export TEST_UPDATE_BSL=1
```

## Retain Test run built artifacts

When investigating tests issues it is sometimes useful to examine the artifacts built when running tests.  Those built using the newer test framework are usually,
built in the %TEMP%\FSharp.Test.Utilities subdirectory.

To tell the test framework to not cleanup these files use the: FSHARP_RETAIN_TESTBUILDS environment variable

Windows:

CMD:

```shell
set FSHARP_RETAIN_TESTBUILDS=1
```

PowerShell:

```shell
$env:FSHARP_RETAIN_TESTBUILDS=1
```

Linux/macOS:

```shell
export FSHARP_RETAIN_TESTBUILDS=1
```

Next, run a build script build (debug or release, desktop or coreclr, depending which baselines you need to update), and test as described [above](#Testing-from-the-command-line). For example:

`./Build.cmd -c Release -testCoreClr` to update Release CoreCLR baselines.

or

`./Build.cmd -c Release -testDesktop` to update Release .NET Framework baselines.

> **Note**
> Please note, that by default, **Release** version of IL baseline tests will be running in CI, so when updating baseline (.bsl) files, make sure to add `-c Release` flag to the build command.


### Parallel execution of tests

Tests utilizing xUnit framework by default run in parallel. If your tests depend on some shared state or are time-critical, you can add the module to predefined `NotThreadSafeResourceCollection` to prevent parallel execution.
For example:
```fsharp
[<Collection(nameof NotThreadSafeResourceCollection)>]
module TimeCritical =
```


### Updating FCS surface area baselines

```bash
$env:TEST_UPDATE_BSL=1
dotnet test tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --filter "SurfaceAreaTest" /p:BUILDING_USING_DOTNET=true
dotnet test tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --filter "SurfaceAreaTest" /p:BUILDING_USING_DOTNET=true
dotnet test tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --filter "SurfaceAreaTest" -c Release /p:BUILDING_USING_DOTNET=true
dotnet test tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --filter "SurfaceAreaTest" -c Release /p:BUILDING_USING_DOTNET=true
```

### Updating ILVerify baselines

These are IL baseline tests for the core assemblies of the compiler (FSharp.Core and FSharp.Compiler.Service). The baselines are located in the `tests/ILVerify` folder and look like:

```
ilverify_FSharp.Compiler.Service_Debug_net9.0.bsl
ilverify_FSharp.Compiler.Service_Debug_netstandard2.0.bsl
ilverify_FSharp.Compiler.Service_Release_net9.0.bsl
ilverify_FSharp.Compiler.Service_Release_netstandard2.0.bsl
ilverify_FSharp.Core_Debug_netstandard2.0.bsl
ilverify_FSharp.Core_Debug_netstandard2.1.bsl
ilverify_FSharp.Core_Release_netstandard2.0.bsl
ilverify_FSharp.Core_Release_netstandard2.1.bsl
```

If you want to update them, either

1. Run the [ilverify.ps1]([url](https://github.com/dotnet/fsharp/blob/main/tests/ILVerify/ilverify.ps1)) script in PowerShell. The script will create `.actual` files. If the differences make sense, replace the original baselines with the actual files.
2. Set the `TEST_UPDATE_BSL` to `1` (please refer to "Updating baselines in tests" section in this file) **and** run `ilverify.ps1` - this will automatically replace baselines. After that, please carefully review the change and push it to your branch if it makes sense.

## Automated Source Code Formatting

Some of the code in this repository is formatted automatically by [Fantomas](https://github.com/fsprojects/fantomas). To format all files use:

```cmd
dotnet fantomas .
```

The formatting is checked automatically by CI:

```cmd
dotnet fantomas . --check
```

At the time of writing only a subset of signature files (`*.fsi`) are formatted. See the settings in `.fantomasignore` and `.editorconfig`.

## Developing the F# tools for Visual Studio

As you would expect, doing this requires both Windows and Visual Studio are installed.

See [Developing on Windows](#Developing-on-Windows) for instructions to install what is needed; it's the same prerequisites.

### Quickly see your changes locally

First, ensure that `VisualFSharpDebug` is the startup project.

Then, use the **f5** or **ctrl+f5** keyboard shortcuts to test your tooling changes. The former will debug a new instance of Visual Studio. The latter will launch a new instance of Visual Studio, but with your changes installed.

Alternatively, you can do this entirely via the command line if you prefer that:

```shell
devenv.exe /rootsuffix RoslynDev
```

### Deploy your changes into a current Visual Studio installation

If you'd like to "run with your changes", you can produce a VSIX and install it into your current Visual Studio instance.

For this, run the following using the VS Developer PowerShell from the repo root:
```shell
VSIXInstaller.exe /u:"VisualFSharp"
VSIXInstaller.exe artifacts\VSSetup\Release\VisualFSharpDebug.vsix
```

It's important to use `Release` if you want to see if your changes have had a noticeable performance impact.

### Troubleshooting a failed build of the tools

You may run into an issue with a somewhat difficult or cryptic error message, like:

> error VSSDK1077: Unable to locate the extensions directory. "ExternalSettingsManager::GetScopePaths failed to initialize PkgDefManager for C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe".

Or hard crash on launch ("Unknown Error").

To fix this, delete these folders:

* `%localappdata%\Microsoft\VisualStudio\<version>_(some number here)RoslynDev`
* `%localappdata%\Microsoft\VisualStudio\<version>_(some number here)`

Where `<version>` corresponds to the latest Visual Studio version on your machine.

## Coding conventions

* Coding conventions vary from file to file

* Format using [the F# style guide](https://learn.microsoft.com/dotnet/fsharp/style-guide/)

* Avoid tick identifiers like `body'`. They are generally harder to read and can't be inspected in the debugger as things stand. Generally use R suffix instead, e.g. `bodyR`. The R can stand for "rewritten" or "result"

* Avoid abbreviations like `bodyty` that are all lowercase. They are really hard to read for newcomers. Use `bodyTy` instead.

* See the compiler docs for common abbreviations

* Don't use `List.iter` and `Array.iter` in the compiler, a `for ... do ...` loop is simpler to read and debug

## Performance and debugging

Use the `Debug` configuration to test your changes locally. It is the default. Do not use the `Release` configuration! Local development and testing of Visual Studio tooling is not designed for the `Release` configuration.

### Benchmarking

Existing compiler benchmarks can be found in `tests\benchmarks\`. The folder contains READMEs describing specific benchmark projects as well as guidelines for creating new benchmarks. There is also `FSharp.Benchmarks.sln` solution containing all the benchmark project and their dependencies.

To exercise the benchmarking infrastructure locally, run:

(Windows)
```cmd
build.cmd -configuration Release -testBenchmarks
```

(Linux/Mac)
```shell
./build.sh --configuration Release --testBenchmarks
```

This is executed in CI as well. It does the following:
- builds all the benchmarking projects
- does smoke testing for fast benchmarks (executes them once to check they don't fail in the runtime)

### Benchmarking and profiling the compiler

**NOTE:** When running benchmarks or profiling compiler, and comparing results with upstream version, make sure:

* Always build both versions of compiler/FCS from source and not use pre-built binaries from SDK (SDK binaries are crossgen'd, which can affect performance).
* To run `Release` build of compiler/FCS.

## Additional resources

The primary technical guide to the core compiler code is [The F# Compiler Technical Guide](https://github.com/dotnet/fsharp/blob/main/docs/index.md). Please read and contribute to that guide.

See the "Debugging The Compiler" section of this [article](https://medium.com/@willie.tetlow/f-mentorship-week-1-36f51d3812d4) for some examples.

## Addendum: configuring a proxy server

If you are behind a proxy server, NuGet client tool must be configured to use it:

See the Nuget config file documentation for use with a proxy server [https://learn.microsoft.com/nuget/reference/nuget-config-file](https://learn.microsoft.com/nuget/reference/nuget-config-file)
