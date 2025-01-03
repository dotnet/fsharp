namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO

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

           Assert.True(result.StdErr.Contains "stack overflow" || result.StdErr.Contains "StackOverflow")

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
