# Benchmarks

## What can be found here

This folder contains code and scripts used for running a selection of performance benchmarks.

### How the tests are used

The existing benchmarks are designed for on-demand local runs, to guide developers in performance improvement efforts and provide limited information during code change discussions.
Each of them assesses a slightly different use case and is run in a different way.

Since there is currently no dedicated hardware setup for running benchmarks in a highly accurate fashion, the results obtained by running them locally have to be treated carefully.
Specifically results obtained on different hardware or in different environments should be treated differently.

### Types of performance tests

Performance tests in this codebase can be broadly put into two groups:
1. tests that measure runtime performance of code produced from F# source code,
2. tests that measure performance of the compilation process itself. This involves any computations required by IDEs, for example type checking.

Group 1. affects end users of programs, while group 2. affects developer experience.

### Directory structure

The code is structured as follows
* `CompiledCodeBenchmarks/` - benchmarks that test compiled code performance.
* `FCSBenchmarks/` - benchmarks of the compiler service itself.
* `FSharp.Benchmarks.Common/` - the library with common code.

### Jupyter notebooks

Some benchmarks are written using F# Notebooks that use the .NET Interactive kernel.
Those can be identified by the `.ipynb` extension.
For instruction on how to run them see https://fsharp.org/use/notebooks/.

### BenchmarkDotNet

Most of the benchmarks use [BenchmarkDotNet](https://benchmarkdotnet.org/) (BDN), a popular benchmarking library for .NET.
It helps avoid common benchmarking pitfalls and provide highly-accurate, repeatable results.

A BDN benchmark is an executable. To run it, simply run `dotnet run %BenchmarkProject.fsproj%` in the benchmark's directory.

## Quickly validating that the benchmarks work

`SmokeTestBenchmarks.ps1` allows to run faster BDN benchmarks with a minimum number of iterations, as a way to verify that the benchmarks still work. This doesn't validate the notebook-based meta-benchmarks.

## Authoring benchmarks

When adding a benchmark, consider:
* choosing an appropriate subdirectory
* adding a README with the following details:
* * what is being measured
* * how to run the benchmark, including any environment requirements
* * an example of the results it produces

Here are the steps for creating benchmarks:

1. Perform a clean build of the compiler and FCS from source (as described in this document, build can be done with `-noVisualStudio` in case if FCS/FSharp.Core is being benchmarked/profiled).

2. Create a benchmark project (in this example, the project will be created in `tests\benchmarks\FCSBenchmarks`).

      ```shell
      cd tests\benchmarks\FCSBenchmarks
      dotnet new console -o FcsBench --name FcsBench -lang F#
      ```

3. Add needed packages and project references.

    ```shell
    cd FcsBench
    dotnet add package BenchmarkDotNet
    dotnet add reference ..\..\..\src\Compiler\FSharp.Compiler.Service.fsproj
    ```

4. Additionally, if you want to test changes to the FSharp.Core (note that the relative path can be different)

     ```shell
     dotnet add reference ..\..\..\src\FSharp.Core\FSharp.Core.fsproj
     ```

    And the following property has to be added to `FcsBench.fsproj`:

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
                  DiagnosticOptions = FSharpDiagnosticOptions.Default
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
                  sourceOpt <- Some <| SourceText.ofString(File.ReadAllText("""C:\Users\vlza\code\fsharp\src\Compiler\Checking\Expressions\CheckExpressions.fs"""))
              | _ -> ()

          [<Benchmark>]
          member _.ParsingCheckExpressionsFs() =
              match checkerOpt, sourceOpt with
              | None, _ -> failwith "no checker"
              | _, None -> failwith "no source"
              | Some(checker), Some(source) ->
                  let results = checker.ParseFile("CheckExpressions.fs",  source, parsingOptions) |> Async.RunSynchronously
                  if results.ParseHadErrors then failwithf "parse had errors: %A" results.Diagnostics

          [<IterationCleanup(Target = "ParsingCheckExpressionsFs")>]
          member _.ParsingCheckExpressionsFsSetup() =
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

     For more detailed information about available BenchmarkDotNet options, please refer to [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/articles/overview.html).

6. Build and run the benchmark.

      ```shell
      dotnet build -c Release
      dotnet run -c Release
      ```

7. You can find results in `.\BenchmarkDotNet.Artifacts\results\` in the current benchmark project directory.

    ```shell
    > ls .\BenchmarkDotNet.Artifacts\results\

        Directory: C:\Users\vlza\code\fsharp\tests\benchmarks\FCSBenchmarks\FcsBench\BenchmarkDotNet.Artifacts\results

    Mode                 LastWriteTime         Length Name
    ----                 -------------         ------ ----
    -a---           4/25/2022  1:42 PM            638 Program.CompilerService-report-github.md
    -a---           4/25/2022  1:42 PM           1050 Program.CompilerService-report.csv
    -a---           4/25/2022  1:42 PM           1169 Program.CompilerService-report.html
    ```

    *-report-github.md can be used to post benchmark results to GitHub issue/PR/discussion or RFC.
    
    *-report.csv can be used for comparison purposes.

    **Example output:**

    ``` ini

    BenchmarkDotNet=v0.13.1, OS=Windows 10.0.25102
    Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
    .NET SDK=6.0.200
      [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT DEBUG
      Job-GDIBXX : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT

    InvocationCount=1  UnrollFactor=1

    ```

    |                    Method |     Mean |   Error |  StdDev |   Median |     Gen 0 |     Gen 1 | Allocated |
    |-------------------------- |---------:|--------:|--------:|---------:|----------:|----------:|----------:|
    | ParsingCheckExpressionsFs | 199.4 ms | 3.84 ms | 9.78 ms | 195.5 ms | 4000.0000 | 1000.0000 |     28 MB |

8. Repeat for any number of changes you would like to test.
9. **Optionally:** benchmark code and results can be included as part of the PR for future reference.
