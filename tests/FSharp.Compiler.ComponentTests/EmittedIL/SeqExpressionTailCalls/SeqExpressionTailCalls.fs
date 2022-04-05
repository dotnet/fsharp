namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module SeqExpressionTailCalls =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=SeqExpressionTailCalls01.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionTailCalls01.exe"	# SeqExpressionTailCalls01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionTailCalls01.fs"|])>]
    let ``SeqExpressionTailCalls01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=SeqExpressionTailCalls02.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionTailCalls02.exe"	# SeqExpressionTailCalls02.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionTailCalls02.fs"|])>]
    let ``SeqExpressionSteppingTest02_fs`` compilation =
        compilation
        |> verifyCompilation
