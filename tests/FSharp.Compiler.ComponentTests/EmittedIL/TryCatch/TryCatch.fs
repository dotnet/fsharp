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

    [<Theory; FileInlineData("StackOverflowRepro.fs")>]
    let ``Stackoverflow prevention`` compilation =
        compilation
        |> getCompilation
        |> setupCompilation
        |> withOptions ["--generate-filter-blocks"]
        |> compileAndRun
        |> shouldSucceed
        |> verifyOutput "System.OperationCanceledException"
