# Development Guide

This document details more advanced options for developing in this codebase. It is not quite necessary to follow it, but it is likely that you'll find something you'll need from here.

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

Install the latest released [Visual Studio](https://www.visualstudio.com/downloads/), as that is what the `main` branch's tools are synced with. Select the following workloads:

* .NET desktop development (also check F# desktop support, as this will install some legacy templates)
* Visual Studio extension development

You will also need the latest .NET 6 SDK installed from [here](https://dotnet.microsoft.com/download/dotnet/6.0).

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

We recommend installing the latest released Visual Studio and using that if you are on Windows. However, if you prefer not to do that, you will need to install the following:

* [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)
* [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)

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

## Updating FSComp.fs, FSComp.resx and XLF

If your changes involve modifying the list of language keywords in any way, (e.g. when implementing a new keyword), the XLF localization files need to be synced with the corresponding resx files. This can be done automatically by running

```shell
pushd src\fsharp\FSharp.Compiler.Service
msbuild FSharp.Compiler.Service.fsproj /t:UpdateXlf
popd
```

This only works on Windows/.NETStandard framework, so changing this from any other platform requires editing and syncing all of the XLF files manually.

## Updating baselines in tests

Some tests use "baseline" files.  There is sometimes a way to update these baselines en-masse in your local build,
useful when some change affects many baselines.  For example, in the `fsharpqa` and `FSharp.Compiler.ComponentTests` tests the baselines
are updated using scripts or utilities that allow the following environment variable to be set:

Windows:

```shell
set TEST_UPDATE_BSL=1
```

Linux/macOS:

```shell
export TEST_UPDATE_BSL=1
```

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

If you'd like to "run with your changes", you can produce a VSIX and install it into your current Visual Studio instance:

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

## Performance and debugging

Use the `Debug` configuration to test your changes locally. It is the default. Do not use the `Release` configuration! Local development and testing of Visual Studio tooling is not designed for the `Release` configuration.

### Writing and running benchmarks

Existing compiler benchmarks can be found in `tests\benchmarks\`.

### Benchmarking and profiling the compiler

**NOTE:** When running benchmarks or profiling compiler, and comparing results with upstream version, make sure:

* Always build both versions of compiler/FCS from source and not use pre-built binaries from SDK (SDK binaries are crossgen'd, which can affect performance).
* To run `Release` build of compiler/FCS.

### Example benchmark setup using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)

1. Perform a clean build of the compiler and FCS from source(as described in this document).
2. Create a benchmark project (in this example, the project will be created in `tests\benchmarks\`).

      ```shell
      cd tests\benchmarks
      dotnet new console -o FcsBench --name FcsBench -lang F#
      ```

3. Add needed packages and project references.

    ```shell
    cd FcsBench
    dotnet add package BenchmarkDotNet
    dotnet add reference ..\..\..\src\fsharp\FSharp.Compiler.Service\FSharp.Compiler.Service.fsproj
    ```

4. Additionally, if you want to test changes to the FSharp.Core

     ```shell
     dotnet add reference ..\..\..\src\fsharp\FSharp.Core\FSharp.Core.fsproj
     ```

    > as well as the following property have to be added to `FcsBench.fsproj`:

    ```xml
    <PropertyGroup>
        <DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>
    </PropertyGroup>
    ```

5. Add a new benchmark for FCS/FSharp.Core by editing `Program.fs`.

      ```fsharp
      open System.IO
      open FSharp.Compiler.CodeAnalysis
      open FSharp.Compiler.Diagnostics
      open FSharp.Compiler.Text
      open BenchmarkDotNet.Attributes
      open BenchmarkDotNet.Running

      [<MemoryDiagnoser>]
      type CompilerService() =
          let mutable checkerOpt = None
          let mutable sourceOpt = None

          let parsingOptions =
              {
                  SourceFiles = [|"CheckExpressions.fs"|]
                  ConditionalDefines = []
                  ErrorSeverityOptions = FSharpDiagnosticOptions.Default
                  LangVersionText = "default"
                  IsInteractive = false
                  LightSyntax = None
                  CompilingFsLib = false
                  IsExe = false
              }

          [<GlobalSetup>]
          member _.Setup() =
              match checkerOpt with
              | None ->
                  checkerOpt <- Some(FSharpChecker.Create(projectCacheSize = 200))
              | _ -> ()

              match sourceOpt with
              | None ->
                  sourceOpt <- Some <| SourceText.ofString(File.ReadAllText("""C:\Users\vlza\code\fsharp\src\fsharp\CheckExpressions.fs"""))
              | _ -> ()


          [<Benchmark>]
          member _.ParsingTypeCheckerFs() =
              match checkerOpt, sourceOpt with
              | None, _ -> failwith "no checker"
              | _, None -> failwith "no source"
              | Some(checker), Some(source) ->
                  let results = checker.ParseFile("CheckExpressions.fs",  source, parsingOptions) |> Async.RunSynchronously
                  if results.ParseHadErrors then failwithf "parse had errors: %A" results.Diagnostics

          [<IterationCleanup(Target = "ParsingTypeCheckerFs")>]
          member _.ParsingTypeCheckerFsSetup() =
              match checkerOpt with
              | None -> failwith "no checker"
              | Some(checker) ->
                  checker.InvalidateAll()
                  checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
                  checker.ParseFile("dummy.fs", SourceText.ofString "dummy", parsingOptions) |> Async.RunSynchronously |> ignore

      [<EntryPoint>]
      let main _ =
          BenchmarkRunner.Run<CompilerService>() |> ignore
          0
      ```

      > For more detailed information about available BenchmarkDotNet options, please refer to [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/articles/overview.html).

6. Build and run the benchmark.

      ```shell
      dotnet build -c Release
      dotnet run -c Release
      ```

7. You can find results in `.\BenchmarkDotNet.Artifacts\results\` in the current benchmark project directory.

    ```shell
    > ls .\BenchmarkDotNet.Artifacts\results\

        Directory: C:\Users\vlza\code\fsharp\tests\benchmarks\FcsBench\BenchmarkDotNet.Artifacts\results

    Mode                 LastWriteTime         Length Name
    ----                 -------------         ------ ----
    -a---           4/25/2022  1:42 PM            638 Program.CompilerService-report-github.md
    -a---           4/25/2022  1:42 PM           1050 Program.CompilerService-report.csv
    -a---           4/25/2022  1:42 PM           1169 Program.CompilerService-report.html
    ```

    > *-report-github.md can be used to post benchmark results to GitHub issue/PR/discussion or RFC.
    >
    >*-report.csv can be used for comparison purposes.

    **Example output:**

    ``` ini

    BenchmarkDotNet=v0.13.1, OS=Windows 10.0.25102
    Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
    .NET SDK=6.0.200
      [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT DEBUG
      Job-GDIBXX : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT

    InvocationCount=1  UnrollFactor=1

    ```

    |               Method |     Mean |   Error |  StdDev |   Median |     Gen 0 |     Gen 1 | Allocated |
    |--------------------- |---------:|--------:|--------:|---------:|----------:|----------:|----------:|
    | ParsingTypeCheckerFs | 199.4 ms | 3.84 ms | 9.78 ms | 195.5 ms | 4000.0000 | 1000.0000 |     28 MB |

8. Repeat for any number of changes you would like to test.

## Additional resources

The primary technical guide to the core compiler code is [The F# Compiler Technical Guide](https://github.com/dotnet/fsharp/blob/main/docs/index.md). Please read and contribute to that guide.

See the "Debugging The Compiler" section of this [article](https://medium.com/@willie.tetlow/f-mentorship-week-1-36f51d3812d4) for some examples.

## Addendum: configuring a proxy server

If you are behind a proxy server, NuGet client tool must be configured to use it:

See the Nuget config file documention for use with a proxy server [https://docs.microsoft.com/en-us/nuget/reference/nuget-config-file](https://docs.microsoft.com/en-us/nuget/reference/nuget-config-file)
