namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO
open System.Runtime.InteropServices

module TryCatch =

    let setupCompilation compilation = 
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> withNoWarn 75 //The command-line option '--generate-filter-blocks' has been deprecated
        |> withNoWarn 52 //The value has been copied to ensure the original is not mutated
        |> asExe
        |> withNoOptimize
        |> withNoInterfaceData
        |> withNoOptimizationData
        |> withNoDebug
        |> ignoreWarnings

    let verifyCompilation compilation =
        setupCompilation compilation
        |> compile
        |> verifyILBaseline

    [<Theory; FileInlineData("ActivePatternRecoverableException.fs")>]
    let ``TryCatch with active pattern`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActivePatternRecoverableException.fs"|],BaselineSuffix = ".generateFilterBlocks")>]
    let ``TryCatch with active pattern and filter blocks switch`` compilation =
        compilation
        |> withOptions ["--generate-filter-blocks"]
        |> verifyCompilation

    [<Theory; FileInlineData("TryWithExplicitGuard.fs")>]
    let ``TryCatch with explicit guard`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TryWithExplicitGuard.fs"|],BaselineSuffix = ".generateFilterBlocks")>]
    let ``TryCatch with explicit guard and filter blocks switch`` compilation =
        compilation
        |> withOptions ["--generate-filter-blocks"]
        |> verifyCompilation

    let private isExpectedStackOverflow isMacOS (result: ExecutionOutput) =
        match result.Outcome with
        | ExitCode exitCode when exitCode <> 0 ->
            result.StdErr.Contains "stack overflow"
            || result.StdErr.Contains "StackOverflow"
            // On macOS, this reproduction can exit with SIGSEGV (128 + 11) before the runtime writes a diagnostic.
            || (isMacOS && exitCode = 139 && result.StdOut = "" && result.StdErr = "")
        | _ -> false

    [<Theory>]
    [<InlineData(true, 139, "", "", true)>]
    [<InlineData(false, 139, "", "", false)>]
    [<InlineData(true, 0, "", "", false)>]
    [<InlineData(true, 1, "", "", false)>]
    [<InlineData(true, 134, "", "", false)>]
    [<InlineData(true, 139, "", "Unhandled exception", false)>]
    [<InlineData(true, 139, "System.OperationCanceledException", "", false)>]
    [<InlineData(false, 134, "", "stack overflow", true)>]
    [<InlineData(false, -1073741571, "", "StackOverflowException", true)>]
    [<InlineData(true, 134, "", "stack overflow", true)>]
    [<InlineData(true, 0, "", "stack overflow", false)>]
    let ``Stackoverflow result classification`` isMacOS exitCode stdout stderr expected =
        let result = { Outcome = ExitCode exitCode; StdOut = stdout; StdErr = stderr }
        Assert.Equal(expected, isExpectedStackOverflow isMacOS result)

    [<Fact>]
    let ``Stackoverflow requires a process exit code`` () =
        for outcome in [ NoExitCode; Failure (System.Exception("Process failed to start")) ] do
            let result = { Outcome = outcome; StdOut = ""; StdErr = "stack overflow" }
            Assert.False(isExpectedStackOverflow true result)

    [<Theory; FileInlineData("StackOverflowRepro.fs")>]
    let ``Stackoverflow reproduction`` compilation =
        let compilationResult = 
            compilation
            |> getCompilation
            |> setupCompilation
            // I cannot just `compileAndRun` this in process now, because it will crash entire test host.
            |> compile

        match compilationResult with
        | CompilationResult.Success ({OutputPath = Some dllFile} as s) ->
           let fsharpCoreFile = typeof<voption<_>>.Assembly.Location
           File.Copy(fsharpCoreFile, Path.Combine(Path.GetDirectoryName(dllFile), Path.GetFileName(fsharpCoreFile)), true)
           let result = CompilerAssert.ExecuteAndReturnResult (dllFile, isFsx=false, deps = s.Dependencies, newProcess=true)
           printfn "%A" result

           Assert.True(isExpectedStackOverflow (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) result, sprintf "%A" result)

        | _ -> failwith (sprintf "%A" compilationResult)

    [<Theory; FileInlineData("StackOverflowRepro.fs")>]
    let ``Stackoverflow prevention`` compilation =
        compilation
        |> getCompilation
        |> setupCompilation
        |> withOptions ["--generate-filter-blocks"]
        |> compileAndRun
        |> shouldSucceed
        |> verifyOutput "System.OperationCanceledException"
